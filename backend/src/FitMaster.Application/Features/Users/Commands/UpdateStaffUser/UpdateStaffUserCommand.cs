using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Users.Commands.UpdateStaffUser;

public record UpdateStaffUserCommand(
    long UserId,
    string FullName,
    string Phone,
    string? Gender,
    string? Password,
    AppRole? Role,
    bool? IsActivated) : IRequest<Result>;
