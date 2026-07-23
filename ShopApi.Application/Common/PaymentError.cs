namespace ShopApi.Application.Common;

public sealed record PaymentError(string Code, string Message)
{
    public static PaymentError InitiationFailed(string reason) =>
        new("payment_initiation_failed", reason);

    public static PaymentError VerificationFailed(string reference) =>
        new("payment_verification_failed", $"Could not verify payment '{reference}'.");

    public static PaymentError AlreadyPaid() =>
        new("already_paid", "This order has already been paid.");
}