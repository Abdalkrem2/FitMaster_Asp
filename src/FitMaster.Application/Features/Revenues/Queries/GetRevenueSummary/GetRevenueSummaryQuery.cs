using MediatR;

namespace FitMaster.Application.Features.Revenues.Queries.GetRevenueSummary;

public record GetRevenueSummaryQuery(DateOnly? From, DateOnly? To) : IRequest<RevenueSummaryDto>;
