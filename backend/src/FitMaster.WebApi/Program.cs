using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitMaster.Application;
using FitMaster.Infrastructure;
using FitMaster.Infrastructure.Persistence.Seeding;
using FitMaster.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Community license: free for organizations with less than $1M annual revenue.
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication - the secret key is read from User Secrets
// ("Jwt:SecretKey"), never from appsettings.json.
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException(
        "Jwt:SecretKey is not set. Configure it via User Secrets: " +
        "dotnet user-secrets set \"Jwt:SecretKey\" \"<a long random string>\"");

// Stripe (online payments) - same fail-fast pattern as Jwt:SecretKey above. Both are
// checked eagerly (unlike Groq/Cloudinary, which fail lazily only when actually used)
// because a missing key here doesn't just disable one feature - the webhook endpoint
// would either be broken outright or, worse, unable to verify signatures at all.
_ = builder.Configuration["Stripe:SecretKey"]
    ?? throw new InvalidOperationException(
        "Stripe:SecretKey is not set. Configure it via User Secrets: " +
        "dotnet user-secrets set \"Stripe:SecretKey\" \"sk_test_...\"");
_ = builder.Configuration["Stripe:WebhookSecret"]
    ?? throw new InvalidOperationException(
        "Stripe:WebhookSecret is not set. Configure it via User Secrets: " +
        "dotnet user-secrets set \"Stripe:WebhookSecret\" \"whsec_...\"");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS - origins allowed to call this API from a browser (the frontend's dev server by default).
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(corsOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// Enums serialize as SCREAMING_SNAKE_CASE strings (e.g. "MUSCLE_GAIN"), matching the
// frontend's existing types - not the default plain-integer System.Text.Json behavior.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper)));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Bootstrap: ensure at least one Admin account exists (see AdminSeeder for
// why this has no hardcoded fallback credentials).
await app.Services.SeedAdminUserAsync();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();
app.UseExceptionHandler(); // must be first, so it wraps everything after it

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "FitMaster API v1"));
}

app.UseCors("Frontend");

// Skipped in Development: the frontend dev server always talks to the plain http
// endpoint, and redirecting it to https breaks CORS outright - a cross-origin
// redirect (http:5035 -> https:7129) fails the browser's CORS check even with the
// ordering above, because the *actual* request (not just the OPTIONS preflight)
// still gets redirected once it's past CORS. Only relevant when the dev server is
// launched on the "https" profile (or an IDE defaults to it) instead of "http".
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
