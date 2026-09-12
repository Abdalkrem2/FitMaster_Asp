using FitMaster.Application.Common.Models;
using FitMaster.Domain.Enums;
using MediatR;

namespace FitMaster.Application.Features.Packages.Commands.UpdatePackage;

public record UpdatePackageCommand(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationInDays,
    PackageStatus Status) : IRequest<Result>;
