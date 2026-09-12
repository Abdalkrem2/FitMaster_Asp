using MediatR;

namespace FitMaster.Application.Features.Payments.Queries.GetMyPayments;

public record GetMyPaymentsQuery(long MemberId) : IRequest<List<PaymentDto>>;
