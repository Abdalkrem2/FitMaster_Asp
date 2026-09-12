using FitMaster.Application.Common.Interfaces;
using FitMaster.Application.Features.Packages.Queries.GetPackageById;
using FitMaster.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Packages.Queries.GetPackagesList;

public class GetPackagesListHandler(IApplicationDbContext db)
    : IRequestHandler<GetPackagesListQuery, List<PackageDto>>
{
    public async Task<List<PackageDto>> Handle(GetPackagesListQuery request, CancellationToken cancellationToken)
    {
        var query = db.Packages.Where(p => !p.Deleted);

        if (!request.IncludeInactive)
        {
            query = query.Where(p => p.Status == PackageStatus.Active);
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new PackageDto(p.Id, p.Name, p.Description, p.Price, p.DurationInDays, p.Status))
            .ToListAsync(cancellationToken);
    }
}
