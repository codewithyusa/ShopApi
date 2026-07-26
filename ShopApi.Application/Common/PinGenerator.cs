namespace ShopApi.Application.Common;

public static class PinGenerator
{
    public static string Generate(int length = 6) =>
        Random.Shared.Next(0, (int)Math.Pow(10, length)).ToString(new string('0', length));
}