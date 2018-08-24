using System;
using System.Collections.Generic;
using SkiaSharp;

namespace EarthLens.Models
{
    public class Chip
    {
        public SKRectI Region { get; }
        public SKColor[][] Pixels { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chip"/> class.
        /// </summary>
        /// <param name="region">The region of the chip.</param>
        /// <param name="pixels">The 2-dimensional array of pixels that represents the chip.</param>
        public Chip(SKRectI region, SKColor[][] pixels)
        {
            Region = region;
            Pixels = pixels;
        }

        /// <summary>
        /// Produces the array of <see cref="Chip"/>s from the specified image.
        /// </summary>
        /// <returns>An array of <see cref="Chip"/>s, each has size less than or equal to 300 x 300.</returns>
        /// <param name="image">The image to use to create chips.</param>
        public static IEnumerable<Chip> FromImage(SKImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException();
            }

            var width = image.Width;
            var height = image.Height;

            var chips = new List<Chip>();

            var extraWidth = width % SharedConstants.DefaultChipWidth;
            var extraHeight = height % SharedConstants.DefaultChipHeight;

            var numWidth = width / SharedConstants.DefaultChipWidth + (extraWidth > 0 ? 1 : 0);
            var numHeight = height / SharedConstants.DefaultChipHeight + (extraHeight > 0 ? 1 : 0);

            for (var i = 0; i < numWidth; i++)
            {
                var x = i * SharedConstants.DefaultChipWidth;
                var subWidth = i + 1 < numWidth || extraWidth == 0 ? SharedConstants.DefaultChipWidth : extraWidth;

                for (var j = 0; j < numHeight; j++)
                {
                    var y = j * SharedConstants.DefaultChipHeight;
                    var subHeight = j + 1 < numHeight || extraHeight == 0
                        ? SharedConstants.DefaultChipHeight
                        : extraHeight;

                    var region = new SKRectI(x, y, x + subWidth, y + subHeight);
                    var subImage = image.Subset(region);
                    var pixels = GetPixelsFromImage(subImage);

                    chips.Add(new Chip(region, pixels));
                }
            }

            return chips.ToArray();
        }

        /// <summary>
        /// Gets the pixels from the specified image.
        /// </summary>
        /// <returns>An array of <see cref="SKColor"/>s that represents the pixels of the image.</returns>
        /// <param name="image">The image of interest.</param>
        private static SKColor[][] GetPixelsFromImage(SKImage image)
        {
            var width = image.Width;
            var height = image.Height;

            var bitmap = SKBitmap.FromImage(image);
            var result = new SKColor[width][];
            for (var i = 0; i < width; i++)
            {
                result[i] = new SKColor[height];
            }

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    result[i][j] = bitmap.GetPixel(i, j);
                }
            }

            return result;
        }
    }
}
