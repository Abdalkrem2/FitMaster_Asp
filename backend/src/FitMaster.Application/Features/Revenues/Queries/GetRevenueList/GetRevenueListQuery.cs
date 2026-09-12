using MediatR;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueList;

public record GetRevenueListQuery(DateOnly? From, DateOnly? To) : IRequest<List<RevenueListItemDto>>;
