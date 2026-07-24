namespace ShopApi.Api.Options;

public class ChapaOptions
{
    public required string SecretKey { get; init; }
    public required string BaseUrl { get; init; } = "https://api.chapa.co/v1";
    public required string CallbackUrl { get; init; }
    public required string ReturnUrl { get; init; }
}