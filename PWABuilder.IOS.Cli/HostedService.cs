using System.Text.Json;
using Microsoft.Extensions.Hosting;
using PWABuilder.IOS.Services.Services;

namespace PWABuilder.IOS.Cli;

internal class HostedService(IOSPackageCreator creator, CliSettings settings, IHostApplicationLifetime lifetime): IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var data = await File.ReadAllBytesAsync(settings.Input, cancellationToken);
        var options = JsonSerializer.Deserialize(data, AppJsonSerializerContext.Default.IOSAppPackageOptions);
        
        if (options == null) throw new Exception("Options is not provided in options.json");
        
        await creator.Create(options.Validate(), settings.Output);
        
        lifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}