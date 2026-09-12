using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitMaster.Application.NutritionGeneration;
using Microsoft.Extensions.Options;

namespace FitMaster.Infrastructure.Ai;

/// <summary>
/// Calls Groq's OpenAI-compatible chat completions endpoint. HttpClient is a typed
/// client registered via AddHttpClient (see DependencyInjection.cs), with base
/// address/timeout/auth header configured there - not a bare `new HttpClient()`.
/// </summary>
public class GroqMealPlanClient(HttpClient httpClient, IOptions<GroqSettings> settings) : IGroqMealPlanClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<string> GetChatCompletionAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Value.ApiKey))
        {
            throw new InvalidOperationException(
                "Groq:ApiKey is not set. Configure it via User Secrets: " +
                "dotnet user-secrets set \"Groq:ApiKey\" \"<your Groq API key>\"");
        }

        var request = new ChatRequest(
            settings.Value.Model,
            [new ChatMessage("user", prompt)],
            Temperature: 0.7,
            MaxCompletionTokens: 1000);

        var response = await httpClient.PostAsJsonAsync(settings.Value.ApiUrl, request, cancellationToken: cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Groq API error: {(int)response.StatusCode} - {body}");
        }

        var parsed = JsonSerializer.Deserialize<ChatResponse>(body, JsonOptions);
        var content = parsed?.Choices?.FirstOrDefault()?.Message?.Content;

        return string.IsNullOrWhiteSpace(content)
            ? throw new HttpRequestException("Groq API returned an empty completion.")
            : content;
    }

    private record ChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] List<ChatMessage> Messages,
        [property: JsonPropertyName("temperature")] double Temperature,
        [property: JsonPropertyName("max_completion_tokens")] int MaxCompletionTokens);

    private record ChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private record ChatResponse([property: JsonPropertyName("choices")] List<ChatChoice>? Choices);

    private record ChatChoice([property: JsonPropertyName("message")] ChatMessage? Message);
}
