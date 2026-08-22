namespace ShopApi.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, CancellationToken ct = default);
}