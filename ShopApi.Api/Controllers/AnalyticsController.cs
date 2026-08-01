using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Analytics.Queries;

namespace ShopApi.Api.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/analytics")]
public class AnalyticsController(IMediator mediator) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAnalyticsSummaryQuery(), ct));

    [HttpGet("daily-sales")]
    public async Task<IActionResult> GetDailySales([FromQuery] int days = 30, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetDailySalesQuery(days), ct));

    [HttpGet("top-selling-products")]
    public async Task<IActionResult> GetTopSellingProducts([FromQuery] int topN = 10, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetTopSellingProductsQuery(topN), ct));

    [HttpGet("coupon-usage")]
    public async Task<IActionResult> GetCouponUsage(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCouponUsageQuery(), ct));
}