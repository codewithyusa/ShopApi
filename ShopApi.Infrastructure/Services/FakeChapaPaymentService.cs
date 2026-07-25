using ShopApi.Application.Interfaces;

namespace ShopApi.Infrastructure.Services;

/// <summary>
/// Stub implementation used until real Chapa credentials are available.
/// Simulates a successful payment every time — swap for ChapaPaymentService later.
/// </summary>
public class FakeChapaPaymentService : IChapaPaymentService
{
    public Task<ChapaInitResult> InitializeAsync(
        string txRef, decimal amount, string email, string firstName, CancellationToken ct)
    {
        // Simulates Chapa's hosted checkout page with a fake local URL.
        var fakeCheckoutUrl = $"http://localhost:5195/api/payments/fake-checkout?txRef={txRef}";
        return Task.FromResult(new ChapaInitResult(true, fakeCheckoutUrl, null));
    }

    public Task<ChapaVerifyResult> VerifyAsync(string txRef, CancellationToken ct)
    {
        // Always reports success — good enough to test the order-status transition.
        return Task.FromResult(new ChapaVerifyResult(true, "success", null, null));
    }
}