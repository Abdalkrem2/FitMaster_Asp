using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Payments.Commands.ProcessPaymentWebhook;
using FitMaster.Application.MembershipProvisioning;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Entities.Payments;
using FitMaster.Domain.Enums;
using FitMaster.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Tests.Payments;

public class ProcessPaymentWebhookHandlerTests
{
    private static FitMasterDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitMasterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FitMasterDbContext(options);
    }

    private static async Task<Payment> SeedPendingPaymentAsync(FitMasterDbContext db, decimal amount = 49.99m)
    {
        var member = new User { Phone = "0700000000", PasswordHash = "hash", FullName = "Ahmad Ali", IsActivated = true };
        db.Users.Add(member);

        var package = new Package { Name = "Gold Monthly", Price = amount, DurationInDays = 30, Status = PackageStatus.Active };
        db.Packages.Add(package);
        await db.SaveChangesAsync();

        var payment = new Payment
        {
            MemberId = member.Id,
            PackageId = package.Id,
            Amount = amount,
            Status = PaymentStatus.Pending,
            GatewayReference = "cs_test_123",
            CreatedAt = DateTime.UtcNow,
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        return payment;
    }

    private static ProcessPaymentWebhookHandler CreateHandler(FitMasterDbContext db, FakePaymentGatewayService gateway)
        => new(db, gateway, new MembershipProvisioningService(db), new NoOpPublisher());

    [Fact]
    public async Task Handle_CompletedOutcome_ProvisionsMembershipAndMarksPaymentCompleted()
    {
        using var db = CreateContext();
        var payment = await SeedPendingPaymentAsync(db);
        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", GatewayPaymentOutcome.Completed, payment.Id),
        };

        var result = await CreateHandler(db, gateway).Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = await db.Payments.SingleAsync(p => p.Id == payment.Id);
        Assert.Equal(PaymentStatus.Completed, updated.Status);
        Assert.NotNull(updated.MembershipId);
        Assert.NotNull(updated.CompletedAt);

        var membership = await db.Memberships.SingleAsync(m => m.Id == updated.MembershipId);
        Assert.Equal(payment.MemberId, membership.MemberId);
        Assert.Equal(MembershipStatus.Active, membership.Status);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), membership.StartDate);
        Assert.Equal(0, membership.Debt); // paid in full through Stripe

        var revenue = await db.Revenues.SingleAsync(r => r.MembershipId == membership.Id);
        Assert.Equal(payment.Amount, revenue.Amount);
    }

    [Fact]
    public async Task Handle_MemberHasActiveMembership_StacksNewMembershipAfterExistingCoverage()
    {
        using var db = CreateContext();
        var payment = await SeedPendingPaymentAsync(db);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existingEndDate = today.AddDays(15);
        db.Memberships.Add(new Membership
        {
            MemberId = payment.MemberId,
            PackageId = payment.PackageId,
            Status = MembershipStatus.Active,
            StartDate = today.AddDays(-15),
            EndDate = existingEndDate,
            Price = payment.Amount,
        });
        await db.SaveChangesAsync();

        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", GatewayPaymentOutcome.Completed, payment.Id),
        };

        await CreateHandler(db, gateway).Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        var updated = await db.Payments.SingleAsync(p => p.Id == payment.Id);
        var newMembership = await db.Memberships.SingleAsync(m => m.Id == updated.MembershipId);
        Assert.Equal(existingEndDate, newMembership.StartDate);
        Assert.Equal(existingEndDate.AddDays(30), newMembership.EndDate);
    }

    [Fact]
    public async Task Handle_DuplicateWebhookDelivery_DoesNotDoubleCreateMembershipOrRevenue()
    {
        using var db = CreateContext();
        var payment = await SeedPendingPaymentAsync(db);
        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", GatewayPaymentOutcome.Completed, payment.Id),
        };

        var handler = CreateHandler(db, gateway);
        var first = await handler.Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);
        // Stripe redelivers the same event (e.g. it didn't get our 200 in time).
        var second = await handler.Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(1, await db.Memberships.CountAsync(m => m.MemberId == payment.MemberId));
        Assert.Equal(1, await db.Revenues.CountAsync());
    }

    [Theory]
    [InlineData(GatewayPaymentOutcome.Failed, PaymentStatus.Failed)]
    [InlineData(GatewayPaymentOutcome.Cancelled, PaymentStatus.Cancelled)]
    public async Task Handle_FailedOrCancelledOutcome_MarksPaymentWithoutTouchingMemberships(
        GatewayPaymentOutcome outcome, PaymentStatus expectedStatus)
    {
        using var db = CreateContext();
        var payment = await SeedPendingPaymentAsync(db);
        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", outcome, payment.Id),
        };

        var result = await CreateHandler(db, gateway).Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = await db.Payments.SingleAsync(p => p.Id == payment.Id);
        Assert.Equal(expectedStatus, updated.Status);
        Assert.Null(updated.MembershipId);
        Assert.Empty(db.Memberships);
        Assert.Empty(db.Revenues);
    }

    [Fact]
    public async Task Handle_IgnoredOutcome_DoesNothing()
    {
        using var db = CreateContext();
        var payment = await SeedPendingPaymentAsync(db);
        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", GatewayPaymentOutcome.Ignored, null),
        };

        var result = await CreateHandler(db, gateway).Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        Assert.True(result.Succeeded);
        var unchanged = await db.Payments.SingleAsync(p => p.Id == payment.Id);
        Assert.Equal(PaymentStatus.Pending, unchanged.Status);
    }

    [Fact]
    public async Task Handle_PaymentNotFound_ReturnsFailure()
    {
        using var db = CreateContext();
        var gateway = new FakePaymentGatewayService
        {
            WebhookResultToReturn = new GatewayWebhookResult("evt_1", GatewayPaymentOutcome.Completed, 999999),
        };

        var result = await CreateHandler(db, gateway).Handle(new ProcessPaymentWebhookCommand("{}", "sig"), CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
