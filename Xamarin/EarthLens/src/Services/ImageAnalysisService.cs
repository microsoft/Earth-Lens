using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EarthLens.Models;
using SkiaSharp;

namespace EarthLens.Services
{
    public static class ImageAnalysisService
    {
        /// <summary>
        /// Analyzes the specified <see cref="SKImage"/> using the specified chip-analyzer function.
        /// </summary>
        /// <param name="image">The specified <see cref="SKImage"/> to analyze.</param>
        /// <param name="chipAnalyzer">The specified chip-analyzer function to analyze a single chip.</param>
        /// <param name="ct">The external <see cref="CancellationToken"/>.</param>
        /// <param name="progressUpdater">The specified function to update the progress bar on the analyzing page.</param>
        /// <returns><see cref="CategoryManager"/>s that represent the results of the chips of the specified <see cref="SKImage"/>.</returns>
        public static IEnumerable<CategoryManager> AnalyzeImage(SKImage image, Func<Chip, CategoryManager> chipAnalyzer,
            CancellationToken ct, Action progressUpdater)
        {
            var chips = Chip.FromImage(image).ToList();
            return chips.AsParallel().WithCancellation(ct).Select(chip =>
            {
                var result = chipAnalyzer(chip);
                progressUpdater();
                return result;
            });
        }
    }
}
