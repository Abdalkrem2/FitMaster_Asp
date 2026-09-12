using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Payments.Commands.CreatePaymentSession;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using FitMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Tests.Payments;

public class CreatePaymentSessionHandlerTests
{
    private static FitMasterDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitMasterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FitMasterDbContext(options);
    }

    [Fact]
    public async Task Handle_ActivePackage_CreatesPendingPaymentAndReturnsCheckoutUrl()
    {
        using var db = CreateContext();
        var member = new User { Phone = "0700000000", PasswordHash = "hash", FullName = "Test Member", IsActivated = true };
        db.Users.Add(member);
        var package = new Package { Name = "Gold Monthly", Price = 49.99m, DurationInDays = 30, Status = PackageStatus.Active };
        db.Packages.Add(package);
        await db.SaveChangesAsync();

        var gateway = new FakePaymentGatewayService
        {
            SessionToReturn = new CheckoutSessionResult("cs_test_abc", "https://checkout.stripe.com/pay/cs_test_abc"),
        };
        var handler = new CreatePaymentSessionHandler(db, gateway);

        var result = await handler.Handle(
            new CreatePaymentSessionCommand(member.Id, package.Id, "https://app/success", "https://app/cancel"),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal("https://checkout.stripe.com/pay/cs_test_abc", result.Value!.CheckoutUrl);

        var payment = await db.Payments.SingleAsync(p => p.Id == result.Value.PaymentId);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(package.Price, payment.Amount);
        Assert.Equal("cs_test_abc", payment.GatewayReference);
    }

    [Fact]
    public async Task Handle_InactivePackage_ReturnsFailureAndCreatesNoPayment()
    {
        using var db = CreateContext();
        var member = new User { Phone = "0700000000", PasswordHash = "hash", FullName = "Test Member", IsActivated = true };
        db.Users.Add(member);
        var package = new Package { Name = "Discontinued", Price = 19.99m, DurationInDays = 30, Status = PackageStatus.Inactive };
        db.Packages.Add(package);
        await db.SaveChangesAsync();

        var handler = new CreatePaymentSessionHandler(db, new FakePaymentGatewayService());

        var result = await handler.Handle(
            new CreatePaymentSessionCommand(member.Id, package.Id, "https://app/success", "https://app/cancel"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Empty(db.Payments);
    }

    [Fact]
    public async Task Handle_PackageNotFound_ReturnsFailure()
    {
        using var db = CreateContext();
        var member = new User { Phone = "0700000000", PasswordHash = "hash", FullName = "Test Member", IsActivated = true };
        db.Users.Add(member);
        await db.SaveChangesAsync();

        var handler = new CreatePaymentSessionHandler(db, new FakePaymentGatewayService());

        var result = await handler.Handle(
            new CreatePaymentSessionCommand(member.Id, PackageId: 999999, "https://app/success", "https://app/cancel"),
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Empty(db.Payments);
    }
}
