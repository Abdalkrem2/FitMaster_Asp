using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Users.Commands.DeleteStaffUser;

public record DeleteStaffUserCommand(long UserId) : IRequest<Result>;
