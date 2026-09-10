using FitMaster.Application.ActivityLogging;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Common.Models;
using FitMaster.Domain.Entities.Identity;
using FitMaster.Domain.Entities.Members;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Members.Commands.CreateMember;

public class CreateMemberHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, ICurrentUserService currentUser, IPublisher publisher)
    : IRequestHandler<CreateMemberCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var phoneTaken = await db.Users.AnyAsync(u => u.Phone == request.Phone, cancellationToken);
        if (phoneTaken)
        {
            return Result<long>.Failure("This phone number is already registered.");
        }

        var memberRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == AppRole.Member, cancellationToken);
        if (memberRole is null)
        {
            memberRole = new Role { RoleName = AppRole.Member };
            db.Roles.Add(memberRole);
        }

        var user = new User
        {
            Phone = request.Phone,
            PasswordHash = passwordHasher.Hash(request.Password),
            FullName = request.FullName,
            IsActivated = true,
            Deleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = [memberRole],
        };
        db.Users.Add(user);

        // MemberProfile shares its primary key with the User (see Phase 1/2),
        // so it must be added after the User's Id is known.
        await db.SaveChangesAsync(cancellationToken);

        var profile = new MemberProfile
        {
            MemberId = user.Id,
            Goal = request.Goal,
            FitnessLevel = request.FitnessLevel,
            SplitType = request.SplitType,
            TrainingStyle = request.TrainingStyle,
            Injuries = request.Injuries ?? [],
            Weight = request.Weight,
            Height = request.Height,
            Age = request.Age,
            HasDiabetes = request.HasDiabetes,
            HasHeartConditions = request.HasHeartConditions,
            HasHypertension = request.HasHypertension,
            Allergies = request.Allergies ?? [],
        };
        db.MemberProfiles.Add(profile);

        await db.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MemberCreatedEvent(user.Id, currentUser.UserId!.Value), cancellationToken);

        return Result<long>.Success(user.Id);
    }
}
