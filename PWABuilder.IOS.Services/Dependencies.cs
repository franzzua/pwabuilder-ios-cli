using Microsoft.Extensions.DependencyInjection;
using PWABuilder.IOS.Services.Services;

namespace PWABuilder.IOS.Services;

public static class Dependencies
{
    public static IServiceCollection AddBuilderServices(this IServiceCollection services)
    {
        services.AddTransient<ImageGenerator>();
        services.AddTransient<IOSPackageCreator>();
        return services;
    }
}