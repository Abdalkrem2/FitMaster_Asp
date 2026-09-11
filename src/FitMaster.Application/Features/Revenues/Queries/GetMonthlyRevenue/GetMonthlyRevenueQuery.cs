using MediatR;

namespace FitMaster.Application.Features.Revenues.Queries.GetMonthlyRevenue;

public record GetMonthlyRevenueQuery(int Year) : IRequest<MonthlyRevenueDto>;
