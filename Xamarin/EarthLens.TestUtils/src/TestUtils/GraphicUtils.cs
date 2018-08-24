using EarthLens.Models;
using SkiaSharp;

namespace EarthLens.Tests.TestUtils
{
    public static class GraphicUtils
    {
        /// <summary>
        /// Generates a 2-D array of <see cref="SKColor"/>s given the specified width and height.
        /// </summary>
        /// <param name="width">The specified width.</param>
        /// <param name="height">The specified height.</param>
        /// <returns>The generated 2-D array of <see cref="SKColor"/>s.</returns>
        public static SKColor[][] GetPixelsByDimensions(int width, int height)
        {
            var bitmap = GetBitmapByDimensions(width, height);
            var result = GetPixelsIn2DFromBitmap(bitmap);
            return result;
        }

        /// <summary>
        /// Generates an <see cref="SKImage"/> given the specified width and height.
        /// </summary>
        /// <param name="width">The specified width.</param>
        /// <param name="height">The specified height.</param>
        /// <returns>The generated <see cref="SKImage"/>.</returns>
        public static SKImage GetImageByDimensions(int width, int height)
        {
            var bitmap = GetBitmapByDimensions(width, height);
            var image = SKImage.FromBitmap(bitmap);
            return image;
        }

        /// <summary>
        /// Generates a <see cref="Chip"/> given the specified left coordinate, top coordinate, width and height.
        /// </summary>
        /// <param name="x">The specified left coordinate.</param>
        /// <param name="y">The specified top coordinate.</param>
        /// <param name="width">The specified width.</param>
        /// <param name="height">The specified height.</param>
        /// <returns>The generated <see cref="Chip"/>.</returns>
        public static Chip GetChipByRegion(int x, int y, int width, int height)
        {
            var region = new SKRectI(x, y, x + width, y + height);
            var pixels = GetPixelsByDimensions(width, height);
            var chip = new Chip(region, pixels);
            return chip;
        }

        /// <summary>
        /// Generates an <see cref="SKBitmap"/> given the specified width and height.
        /// </summary>
        /// <param name="width">The specified width.</param>
        /// <param name="height">The specified height.</param>
        /// <returns>The generated <see cref="SKBitmap"/>.</returns>
        private static SKBitmap GetBitmapByDimensions(int width, int height)
        {
            var bitmap = new SKBitmap(width, height, false);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    bitmap.SetPixel(i, j, new SKColor(
                        (byte)(i % SharedConstants.DefaultChipWidth % 256),
                        (byte)(j % SharedConstants.DefaultChipHeight % 256),
                        0));
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Retrieves the 2-D array of <see cref="SKColor"/>s from the specified <see cref="SKBitmap"/>.
        /// </summary>
        /// <param name="bitmap">The specified <see cref="SKBitmap"/>.</param>
        /// <returns>The generated 2-D array of <see cref="SKColor"/>s.</returns>
        private static SKColor[][] GetPixelsIn2DFromBitmap(SKBitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

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
