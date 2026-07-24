namespace ShopApi.Application.Interfaces;

public record ChapaInitResult(bool Success, string? CheckoutUrl, string? Error);
public record ChapaVerifyResult(bool Success, string Status, decimal? Amount, string? Error);

public interface IChapaPaymentService
{
    Task<ChapaInitResult> InitializeAsync(
        string txRef, decimal amount, string email, string firstName, CancellationToken ct);

    Task<ChapaVerifyResult> VerifyAsync(string txRef, CancellationToken ct);
}