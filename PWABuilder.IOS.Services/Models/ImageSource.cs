﻿using System.IO.Compression;
using PWABuilder.IOS.Services.Extensions;

namespace PWABuilder.IOS.Services.Models
{
    /// <summary>
    /// An image source used for the app package. Contains 2 potential sources of an image:
    /// an image URI specified in the web app manifest,
    /// an image entry in a zip file generated by the PWABuilder app image service.
    /// </summary>
    public class ImageSource
    {
        /// <summary>
        /// Gets the URI of the image taken from the PWA's web manifest.
        /// This has priority 1.
        /// </summary>
        public Uri? WebManifestSource { get; set; }

        /// <summary>
        /// Gets the zip entry of the image that was generated on behalf of the user.
        /// This is the lowest priority source, priority 2.
        /// </summary>
        public ZipArchiveEntry? GeneratedImageSource { get; set; }

        /// <summary>
        /// Gets the target file name.
        /// </summary>
        public string TargetFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the size of the image source.
        /// </summary>
        public ImageTargetSize Size { get; set; }

        /// <summary>
        /// Creates an ImageSource for the specified scale set.
        /// </summary>
        /// <param name="imageSetType"></param>
        /// <param name="scale"></param>
        /// <param name="imageOptions"></param>
        /// <param name="webManifest"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static ImageSource From(ImageTargetSize targetSize, WebAppManifestContext webManifest, ImageGeneratorServiceZipFile zip)
        {
            return new ImageSource
            {
                Size = targetSize,
                TargetFileName = targetSize.ToFileName() + ".png",
                WebManifestSource = webManifest.GetIconUriFromTargetSize(targetSize),
                GeneratedImageSource = zip.GetTargetSize(targetSize)
            };
        }
    }
}
