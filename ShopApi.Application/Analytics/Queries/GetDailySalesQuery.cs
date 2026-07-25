using MediatR;
using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Analytics.Queries;

public record GetDailySalesQuery(int Days = 30) : IRequest<List<DailySalesDto>>;