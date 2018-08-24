using System;
using System.Collections.Generic;
using EarthLens.Models;
using SkiaSharp;

namespace EarthLens
{
    public static class GraphicService
    {
        /// <summary>
        /// Draws the specified <see cref="Observation"/>s on the specified <see cref="SKImage"/>.
        /// </summary>
        /// <param name="rawImage">The specified <see cref="SKImage"/>.</param>
        /// <param name="observations">The specified <see cref="Observation"/>s.</param>
        /// <returns>An <see cref="SKImage"/> with the raw <see cref="SKImage"/> and with all <see cref="Observation"/>s.</returns>
        public static SKImage DrawObservations(SKImage rawImage, IEnumerable<Observation> observations)
        {
            if (rawImage == null || observations == null)
            {
                throw new ArgumentNullException();
            }

            using (var surface = SKSurface.Create(rawImage.Width, rawImage.Height, SKImageInfo.PlatformColorType,
                SKAlphaType.Premul))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                // set up drawing tools
                using (var paint = new SKPaint())
                {
                    paint.StrokeWidth = SharedConstants.BoundingBoxWidthExport;
                    paint.IsStroke = true;

                    canvas.DrawImage(rawImage, new SKPoint(0f, 0f));

                    foreach (var observation in observations)
                    {
                        // create the path
                        using (var path = new SKPath())
                        {
                            var rect = observation.BoundingBox;
                            path.AddRect(observation.BoundingBox);
                            paint.Color = observation.Category.Color;
                            canvas.DrawPath(path, paint);
                        }
                    }
                }

                return surface.Snapshot();
            }
        }
    }
}