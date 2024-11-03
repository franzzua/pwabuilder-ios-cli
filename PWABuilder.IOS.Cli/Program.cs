using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWABuilder.IOS.Cli;
using PWABuilder.IOS.Services;
using PWABuilder.IOS.Services.Models;

var settings = Cli.ParseArgs(args);
if (settings == null) return;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddBuilderServices();
builder.Services.AddHostedService<HostedService>();
var section = builder.Configuration.GetSection("AppSettings");
var appSettings = new AppSettings
{
    IOSSourceCodePath = section["IOSSourceCodePath"]!,
    ImageGeneratorApiUrl = section["ImageGeneratorApiUrl"]! 
};
builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton(settings);

var app = builder.Build();

await app.StartAsync();

Console.WriteLine("PWA successfully built, chao!");