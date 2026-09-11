using MediatR;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueStats;

public record GetRevenueStatsQuery : IRequest<RevenueStatsDto>;
