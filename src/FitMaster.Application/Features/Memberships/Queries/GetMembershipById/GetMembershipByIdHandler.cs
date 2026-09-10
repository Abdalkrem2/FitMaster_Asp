using FitMaster.Application.Common.Exceptions;
using FitMaster.Application.Common.Interfaces;
using FitMaster.Domain.Entities.Memberships;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Memberships.Queries.GetMembershipById;

public class GetMembershipByIdHandler(IApplicationDbContext db)
    : IRequestHandler<GetMembershipByIdQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(GetMembershipByIdQuery request, CancellationToken cancellationToken)
    {
        var membership = await db.Memberships
            .Where(m => m.Id == request.Id)
            .Select(m => new MembershipDto(
                m.Id,
                m.MemberId,
                m.Member.FullName,
                m.PackageId,
                m.Package.Name,
                m.Status,
                m.StartDate,
                m.EndDate,
                m.Price,
                m.Debt,
                m.Description))
            .FirstOrDefaultAsync(cancellationToken);

        return membership ?? throw new NotFoundException(nameof(Membership), request.Id);
    }
}
