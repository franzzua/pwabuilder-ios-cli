using Microsoft.Extensions.Logging;
using PWABuilder.IOS.Services.Extensions;
using PWABuilder.IOS.Services.Models;

namespace PWABuilder.IOS.Services.Services;

public class IOSPackageCreator(
    ImageGenerator imageGenerator,
    AppSettings appSettings,
    ILogger<IOSPackageCreator> logger)
{

    /// <summary>
    /// Generates an iOS package.
    /// </summary>
    /// <param name="options">The package creation options.</param>
    /// <param name="outputDir">Where write files</param>
    public async Task Create(IOSAppPackageOptions.Validated options, string outputDir)
    {
        try
        {

            // Make a copy of the iOS source code.
            new DirectoryInfo(appSettings.IOSSourceCodePath).CopyContents(new DirectoryInfo(outputDir));

            // Create any missing images for the iOS template.
            // This should be done before project.ApplyChanges(). Otherwise, it'll attempt to write the images to the "pwa-shell" directory, which no longer exists after ApplyChanges().
            await imageGenerator.Generate(options, WebAppManifestContext.From(options.Manifest, options.ManifestUri), outputDir);

            // Update the source files with the real values from the requested PWA
            var project = new XcodePwaShellProject(options, outputDir);
            project.Load();
            await project.ApplyChanges();

        }
        catch (Exception error)
        {
            logger.LogError(error, "Error generating iOS package");
            throw;
        }
    }

}