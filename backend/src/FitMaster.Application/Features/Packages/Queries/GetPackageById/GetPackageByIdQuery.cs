using FitMaster.Application.Features.Packages.Queries.GetPackageById;
using MediatR;

namespace FitMaster.Application.Features.Packages.Queries.GetPackageById;

public record GetPackageByIdQuery(long Id) : IRequest<PackageDto>;
