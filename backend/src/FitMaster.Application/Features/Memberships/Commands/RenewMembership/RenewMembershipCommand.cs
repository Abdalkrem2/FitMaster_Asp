using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Memberships.Commands.RenewMembership;

/// <summary>
/// Extends an existing membership by its package's duration again, starting
/// either today or right after the current end date (whichever the caller
/// specifies) - and reactivates it if it had expired.
/// </summary>
public record RenewMembershipCommand(long MembershipId, DateOnly? StartDate) : IRequest<Result>;
