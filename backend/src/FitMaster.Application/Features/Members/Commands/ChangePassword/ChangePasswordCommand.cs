using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Members.Commands.ChangePassword;

public record ChangePasswordCommand(long UserId, string CurrentPassword, string NewPassword) : IRequest<Result>;
