using System;
using System.IO;
using SkiaSharp;

namespace EarthLens.Services
{
    public static class ImageEncodingService
    {
        /// <summary>
        /// Converts a UIImage to a Base64 string, using minimal suppression via JPEG conversion
        /// <param name="image">The image being converted.</param>
        /// </summary>
        public static string SKImageToBase64(SKImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException();
            }

            var data = image.Encode(SharedConstants.EncodingFormat, SharedConstants.EncodingQuality).ToArray();
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Converts a base64 string to a SKImage, using minimal suppression via JPEG conversion
        /// <param name="base64">The base64 string being converted.</param>
        /// </summary>
        public static SKImage Base64ToSKImage(string base64)
        {
            var data = Convert.FromBase64String(base64);
            var stream = new MemoryStream(data);
            var skData = SKData.Create(stream);
            stream.Dispose();
            return SKImage.FromEncodedData(skData);
        }
    }
}