using MediatR;
using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Analytics.Queries;

public record GetAnalyticsSummaryQuery : IRequest<AnalyticsSummaryDto>;