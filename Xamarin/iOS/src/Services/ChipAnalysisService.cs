using System;
using System.IO;
using System.Linq;
using EarthLens.Models;
using EarthLens.Services;

namespace EarthLens.iOS.Services
{
    public static class ChipAnalysisService
    {
        /// <summary>
        /// Analyzes the specified <see cref="Chip"/> using the machine learning model.
        /// </summary>
        /// <param name="chip">The specified <see cref="Chip"/> of default size.</param>
        /// <returns>A <see cref="CategoryManager"/> that contains all <see cref="Category"/>s and <see cref="Observation"/>s detected in the specified <see cref="Chip"/>.</returns>
        public static CategoryManager AnalyzeChip(Chip chip)
        {
            if (chip == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                using (var mlMultiArray = chip.ToMLMultiArray())
                using (var model = new CoreMLModel())
                {
                    var categoryManager = model.Predict(mlMultiArray);

                    var observations = categoryManager.Observations;
                    var observationList = observations.ToList();
                    PostProcessingService.ProcessChipResults(chip.Region.Location, observationList);

                    return categoryManager;
                }
            }
            catch (FileNotFoundException)
            {
                return new CategoryManager();
            }
        }
    }
}