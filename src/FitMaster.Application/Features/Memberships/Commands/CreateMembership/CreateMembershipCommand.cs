using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Memberships.Commands.CreateMembership;

public record CreateMembershipCommand(
    long MemberId,
    long PackageId,
    DateOnly StartDate,
    decimal? Price,
    decimal? Debt,
    string? Description) : IRequest<Result<long>>;
