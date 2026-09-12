using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Memberships.Commands.CreateMembership;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Memberships;
using FitMaster.Domain.Enums;
using FitMaster.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Tests.Features.Memberships.CreateMembership;

public class CreateMembershipHandlerTests
{
    private static FitMasterDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitMasterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FitMasterDbContext(options);
    }

    private static async Task<(User member, Package package)> SeedMemberAndPackageAsync(
        FitMasterDbContext db, int packageDurationDays, decimal packagePrice)
    {
        var member = new User
        {
            Phone = "0700000000",
            PasswordHash = "hash",
            FullName = "Test Member",
            IsActivated = true,
        };
        db.Users.Add(member);

        var package = new Package
        {
            Name = "Test Package",
            Price = packagePrice,
            DurationInDays = packageDurationDays,
            Status = PackageStatus.Active,
        };
        db.Packages.Add(package);

        await db.SaveChangesAsync();
        return (member, package);
    }

    [Fact]
    public async Task Handle_MemberHasActiveMembership_QueuesNewMembershipAfterExistingCoverageEnds()
    {
        // Arrange
        using var db = CreateContext();
        var (member, newPackage) = await SeedMemberAndPackageAsync(db, packageDurationDays: 365, packagePrice: 1200);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existingEndDate = today.AddDays(30);

        db.Memberships.Add(new Membership
        {
            MemberId = member.Id,
            PackageId = newPackage.Id,
            Status = MembershipStatus.Active,
            StartDate = today.AddDays(-335),
            EndDate = existingEndDate,
            Price = 1200,
        });
        await db.SaveChangesAsync();

        var handler = new CreateMembershipHandler(db, new FakeCurrentUserService(1), new NoOpPublisher());
        var command = new CreateMembershipCommand(member.Id, newPackage.Id, today, null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        var created = await db.Memberships.SingleAsync(m => m.Id == result.Value);
        Assert.Equal(existingEndDate, created.StartDate);
        Assert.Equal(existingEndDate.AddDays(365), created.EndDate);
    }

    [Fact]
    public async Task Handle_MemberHasNoActiveMembership_StartsToday()
    {
        // Arrange
        using var db = CreateContext();
        var (member, package) = await SeedMemberAndPackageAsync(db, packageDurationDays: 365, packagePrice: 1200);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var handler = new CreateMembershipHandler(db, new FakeCurrentUserService(1), new NoOpPublisher());
        var command = new CreateMembershipCommand(member.Id, package.Id, today, null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        var created = await db.Memberships.SingleAsync(m => m.Id == result.Value);
        Assert.Equal(today, created.StartDate);
        Assert.Equal(today.AddDays(365), created.EndDate);
    }

    [Fact]
    public async Task Handle_ExistingActiveMembershipAlreadyExpired_StartsToday()
    {
        // Arrange
        using var db = CreateContext();
        var (member, package) = await SeedMemberAndPackageAsync(db, packageDurationDays: 365, packagePrice: 1200);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        db.Memberships.Add(new Membership
        {
            MemberId = member.Id,
            PackageId = package.Id,
            Status = MembershipStatus.Active,
            StartDate = today.AddDays(-400),
            EndDate = today.AddDays(-35), // already expired
            Price = 1200,
        });
        await db.SaveChangesAsync();

        var handler = new CreateMembershipHandler(db, new FakeCurrentUserService(1), new NoOpPublisher());
        var command = new CreateMembershipCommand(member.Id, package.Id, today, null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        var created = await db.Memberships.SingleAsync(m => m.Id == result.Value);
        Assert.Equal(today, created.StartDate);
        Assert.Equal(today.AddDays(365), created.EndDate);
    }

    private sealed class FakeCurrentUserService(long userId) : ICurrentUserService
    {
        public long? UserId { get; } = userId;
        public bool IsAuthenticated => true;
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;
    }
}
