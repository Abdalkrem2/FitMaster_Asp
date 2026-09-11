using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.UpdateMemberIdentity;

/// <summary>Updates a member's phone/name - separate from UpdateMemberProfileCommand,
/// which only touches the fitness/health MemberProfile fields.</summary>
public record UpdateMemberIdentityCommand(long MemberId, string FullName, string Phone) : IRequest<Result>;
