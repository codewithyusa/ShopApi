using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ShopApi.Application.Interfaces;

namespace ShopApi.Infrastructure.Services;

public class ChapaOptions
{
    public required string SecretKey { get; init; }
    public string BaseUrl { get; init; } = "https://api.chapa.co/v1";
    public required string CallbackUrl { get; init; }
    public required string ReturnUrl { get; init; }
}

public class ChapaPaymentService(HttpClient http, IOptions<ChapaOptions> options) : IChapaPaymentService
{
    private readonly ChapaOptions _opts = options.Value;

    public async Task<ChapaInitResult> InitializeAsync(
        string txRef, decimal amount, string email, string firstName, CancellationToken ct)
    {
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _opts.SecretKey);

        var payload = new
        {
            amount = amount.ToString("F2"),
            currency = "ETB",
            email,
            first_name = firstName,
            tx_ref = txRef,
            callback_url = _opts.CallbackUrl,
            return_url = _opts.ReturnUrl
        };

        var response = await http.PostAsJsonAsync($"{_opts.BaseUrl}/transaction/initialize", payload, ct);
        var body = await response.Content.ReadFromJsonAsync<ChapaInitResponse>(cancellationToken: ct);

        if (!response.IsSuccessStatusCode || body?.Status != "success")
            return new ChapaInitResult(false, null, body?.Message ?? "Chapa initialization failed.");

        return new ChapaInitResult(true, body.Data?.CheckoutUrl, null);
    }

    public async Task<ChapaVerifyResult> VerifyAsync(string txRef, CancellationToken ct)
    {
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _opts.SecretKey);

        var response = await http.GetAsync($"{_opts.BaseUrl}/transaction/verify/{txRef}", ct);
        var body = await response.Content.ReadFromJsonAsync<ChapaVerifyResponse>(cancellationToken: ct);

        if (!response.IsSuccessStatusCode || body?.Status != "success")
            return new ChapaVerifyResult(false, "failed", null, body?.Message ?? "Verification failed.");

        var paymentStatus = body.Data?.Status ?? "failed";
        return new ChapaVerifyResult(true, paymentStatus, body.Data?.Amount, null);
    }

    private class ChapaInitResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ChapaInitData? Data { get; set; }
    }

    private class ChapaInitData
    {
        public string? CheckoutUrl { get; set; }
    }

    private class ChapaVerifyResponse
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public ChapaVerifyData? Data { get; set; }
    }

    private class ChapaVerifyData
    {
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
    }
}