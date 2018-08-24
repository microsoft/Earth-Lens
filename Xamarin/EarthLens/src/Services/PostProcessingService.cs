using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EarthLens.ClassExtensions;
using EarthLens.Models;
using SkiaSharp;

namespace EarthLens.Services
{
    public static class PostProcessingService
    {
        /// <summary>
        /// Selects observations of the specified <see cref="CategoryManager"/>s that are meaningful to users using
        /// non-maximum suppression algorithm.
        /// </summary>
        /// <returns>The selected <see cref="Observation"/>s.</returns>
        /// <param name="categoryManagers">The specified <see cref="CategoryManager"/>s for a <see cref="Chip"/>.</param>
        /// <param name="iouThreshold">The threshold for IOU (intersection over union).</param>
        /// <param name="confidenceThreshold">The threshold for low-confidence pruning.</param>
        /// <param name="ct">The external <see cref="CancellationToken"/>.</param>
        public static IEnumerable<Observation> SelectObservations(IEnumerable<CategoryManager> categoryManagers, double
            iouThreshold, double confidenceThreshold, CancellationToken ct)
        {
            var selected = categoryManagers.AsParallel().WithCancellation(ct).SelectMany(categoryManager =>
                SelectObservations(categoryManager, iouThreshold, confidenceThreshold));

            return NonMaximumSuppression(selected, iouThreshold, confidenceThreshold);
        }

        /// <summary>
        /// Scales and translates <see cref="Observation"/>s from a <see cref="Chip"/>.
        /// </summary>
        /// <param name="offset">The size of the specified <see cref="Chip"/>.</param>
        /// <param name="observations">The specified <see cref="Observation"/>s from a <see cref="Chip"/>.</param>
        public static void ProcessChipResults(SKPointI offset, IEnumerable<Observation> observations)
        {
            if (observations == null)
            {
                throw new ArgumentNullException();
            }

            var size = new SKSizeI(SharedConstants.DefaultChipWidth, SharedConstants.DefaultChipHeight);

            foreach (var observation in observations)
            {
                observation.ScaleAndTranslate(size, offset);
            }
        }

        /// <summary>
        /// Applies the non-maximum suppression algorithm to the specified <see cref="Observation"/>s using the
        /// specified IOU (intersection over union) threshold and confidence threshold.
        /// </summary>
        /// <param name="observations">The specified <see cref="Observation"/>s.</param>
        /// <param name="iouThreshold">The specified threshold for IOU (intersection over union).</param>
        /// <param name="confidenceThreshold">The specified threshold for low-confidence pruning.</param>
        /// <returns><see cref="Observation"/>s selected according to the specified thresholds.</returns>
        public static IEnumerable<Observation> NonMaximumSuppression(IEnumerable<Observation> observations, double
            iouThreshold, double confidenceThreshold)
        {
            var observationList = observations.ToList();
            var sorted = SortObservationsByConfidence(observationList);
            var selected = new List<Observation>();
            
            foreach (var observationA in sorted)
            {
                if (observationA.Confidence < confidenceThreshold)
                {
                    break;
                }

                var shouldSelect = true;

                foreach (var observationB in selected)
                {
                    if (!(SKRectExtensions.IntersectionOverUnion(observationA.BoundingBox, observationB.BoundingBox) >
                          iouThreshold)) continue;
                    shouldSelect = false;
                    break;
                }

                if (shouldSelect)
                {
                    selected.Add(observationA);
                }
            }

            return selected;
        }

        /// <summary>
        /// Selects observations of the specified <see cref="CategoryManager"/> that are meaningful to users using
        /// non-maximum suppression algorithm.
        /// </summary>
        /// <returns>The selected <see cref="Observation"/>s.</returns>
        /// <param name="categoryManager">A <see cref="CategoryManager"/> for a chip.</param>
        /// <param name="iouThreshold">The threshold for IOU (intersection over union).</param>
        /// <param name="confidenceThreshold">The threshold for low-confidence pruning.</param>
        private static IEnumerable<Observation> SelectObservations(CategoryManager categoryManager, double iouThreshold,
            double confidenceThreshold)
        {
            var categories = categoryManager.Categories;
            return categories.AsParallel().SelectMany(category =>
                NonMaximumSuppression(category.Observations, iouThreshold, confidenceThreshold));
        }

        /// <summary>
        /// Sorts the specified <see cref="Observation"/>s by the confidence value in the descending order.
        /// </summary>
        /// <param name="observations">The specified <see cref="Observation"/>s.</param>
        /// <returns>The sorted <see cref="Observation"/>s.</returns>
        private static IEnumerable<Observation> SortObservationsByConfidence(IEnumerable<Observation> observations)
        {
            var observationList = observations.ToList();
            observationList.Sort((a, b) => (a.Confidence > b.Confidence ? -1 : 1));
            return observationList;
        }
    }
}
