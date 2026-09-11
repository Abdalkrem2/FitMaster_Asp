using MediatR;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenuePeriod;

public record GetRevenuePeriodQuery(DateOnly Start, DateOnly End) : IRequest<RevenuePeriodDto>;
