using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Users.Commands.CreateStaffUser;

/// <summary>
/// Creates an Employee or Admin account. Admin-only (see UsersController) -
/// staff accounts are never self-registered. For Members, use
/// FitMaster.Application.Features.Members.Commands.CreateMember instead.
/// </summary>
public record CreateStaffUserCommand(
    string Phone,
    string Password,
    string FullName,
    AppRole Role) : IRequest<Result<long>>;
