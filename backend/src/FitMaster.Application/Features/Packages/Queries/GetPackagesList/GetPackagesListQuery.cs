using FitMaster.Application.Features.Packages.Queries.GetPackageById;
using MediatR;

namespace FitMaster.Application.Features.Packages.Queries.GetPackagesList;

public record GetPackagesListQuery(bool IncludeInactive = false) : IRequest<List<PackageDto>>;
