using FitMaster.Application.Common.Models;
using MediatR;

namespace FitMaster.Application.Features.Packages.Commands.CreatePackage;

public record CreatePackageCommand(
    string Name,
    string? Description,
    decimal Price,
    int DurationInDays) : IRequest<Result<long>>;
