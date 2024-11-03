using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.PWABuilder.IOS.Web.Models;
using Microsoft.PWABuilder.IOS.Web.Services;
using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PWABuilder.IOS.Services.Extensions;
using PWABuilder.IOS.Services.Models;
using PWABuilder.IOS.Services.Services;
using SystemFile = System.IO.File;


namespace Microsoft.PWABuilder.IOS.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PackagesController(
        IOSPackageCreator packageCreator,
        AnalyticsService analytics,
        TempDirectory temp,
        IOptions<AppSettings> appSettings,
        ILogger<PackagesController> logger) : ControllerBase
    {

        [HttpPost]
        public async Task<FileResult> Create(IOSAppPackageOptions options)
        {
            AnalyticsInfo analyticsInfo = new();

            if (HttpContext?.Request.Headers != null)
            {
                analyticsInfo.platformId = HttpContext.Request.Headers.TryGetValue("platform-identifier", out var id) ? id.ToString() : null;
                analyticsInfo.platformIdVersion = HttpContext.Request.Headers.TryGetValue("platform-identifier-version", out var version) ? version.ToString() : null;
                analyticsInfo.correlationId = HttpContext.Request.Headers.TryGetValue("correlation-id", out var corrId) ? corrId.ToString() : null;
                analyticsInfo.referrer = HttpContext.Request.Query.TryGetValue("ref", out var referrer) ? referrer.ToString() : null;
            }

            try
            {
                var optionsValidated = ValidateOptions(options);
                var outputDir = temp.CreateDirectory($"ios-package-{Guid.NewGuid()}");
                await packageCreator.Create(optionsValidated, outputDir);
                // Zip it all up.
                var zipFile = CreateZip(outputDir);
                var packageBytes = await SystemFile.ReadAllBytesAsync(zipFile);
                analytics.Record(optionsValidated.Url.ToString(), success: true, optionsValidated, analyticsInfo, error: null);
                return this.File(packageBytes, "application/zip", $"{options.Name}-ios-app-package.zip");
            }
            catch (Exception error)
            {
                analytics.Record(options.Url ?? "https://EMPTY_URL", success: false, null, analyticsInfo, error: error.ToString());
                throw;
            }
            finally
            {
                temp.CleanUp();
            }
        }

        private IOSAppPackageOptions.Validated ValidateOptions(IOSAppPackageOptions options)
        {
            try
            {
                return options.Validate();
            }
            catch (Exception error)
            {
                logger.LogError(error, "Invalid package options");
                throw;
            }
        }
        
        private string CreateZip(string outputDir)
        {
            var zipFilePath = temp.CreateFile();
            using var zipFile = SystemFile.Create(zipFilePath);
            using var zipArchive = new ZipArchive(zipFile, ZipArchiveMode.Create);
            zipArchive.CreateEntryFromFile(appSettings.Value.NextStepsPath, "next-steps.html");
            zipArchive.CreateEntryFromDirectory(outputDir, "src");
            return zipFilePath;
        }
    }
}
