using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Phone, string Password) : IRequest<Result<LoginResponse>>;
