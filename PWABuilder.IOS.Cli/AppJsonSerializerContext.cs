using System.Text.Json.Serialization;
using PWABuilder.IOS.Services.Models;

namespace PWABuilder.IOS.Cli
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(IOSAppPackageOptions))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}