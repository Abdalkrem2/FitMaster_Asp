using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Packages.Queries.GetPackageById;

public class GetPackageByIdHandler(IApplicationDbContext db) : IRequestHandler<GetPackageByIdQuery, PackageDto>
{
    public async Task<PackageDto> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var package = await db.Packages
            .Where(p => p.Id == request.Id && !p.Deleted)
            .Select(p => new PackageDto(p.Id, p.Name, p.Description, p.Price, p.DurationInDays, p.Status))
            .FirstOrDefaultAsync(cancellationToken);

        return package ?? throw new NotFoundException(nameof(Domain.Entities.Memberships.Package), request.Id);
    }
}
