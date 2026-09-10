using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Phone, string Password, string FullName)
    : IRequest<Result<RegisterResponse>>;
