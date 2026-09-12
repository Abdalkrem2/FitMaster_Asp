using FitMaster.Domain.Enums;

namespace FitMaster.Application.Features.Packages.Queries.GetPackageById;

public record PackageDto(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationInDays,
    PackageStatus Status);
