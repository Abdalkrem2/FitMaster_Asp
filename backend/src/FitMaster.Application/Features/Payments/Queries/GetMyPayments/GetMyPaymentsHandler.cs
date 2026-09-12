using FitMaster.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitMaster.Application.Features.Payments.Queries.GetMyPayments;

public class GetMyPaymentsHandler(IApplicationDbContext db) : IRequestHandler<GetMyPaymentsQuery, List<PaymentDto>>
{
    public async Task<List<PaymentDto>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        return await db.Payments
            .Where(p => p.MemberId == request.MemberId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto(
                p.Id,
                p.PackageId,
                p.Package.Name,
                p.Amount,
                p.Status,
                p.MembershipId,
                p.CreatedAt,
                p.CompletedAt))
            .ToListAsync(cancellationToken);
    }
}
