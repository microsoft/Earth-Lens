using System;
using System.Collections.Generic;
using EarthLens.iOS.Services;
using EarthLens.Tests.TestUtils;
using CoreML;
using Foundation;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.iOS.Services
{
    [TestFixture]
    public class PreProcessingServiceTests
    {
        private const double Tolerance = 1e-6;

        private static readonly IList<NSNumber> ColorMapping = PreProcessingService.ColorMapping;

        [Test]
        public void TestToMLMultiArray_DefaultDimensions()
        {
            var chip = GraphicUtils.GetChipByRegion(0, 0, SharedConstants.DefaultChipWidth,
                SharedConstants.DefaultChipHeight);
            var result = chip.ToMLMultiArray();

            Assert.AreEqual(new nint[] {1, 1, 3, SharedConstants.DefaultChipWidth, SharedConstants.DefaultChipHeight},
                result.Shape);
            
            for (var i = 0; i < SharedConstants.DefaultChipWidth; i++)
            {
                for (var j = 0; j < SharedConstants.DefaultChipHeight; j++)
                {
                    AssertMLMultiArrayEntry(chip.Pixels[i][j], result, i, j);
                }
            }
        }

        [TestCase(SharedConstants.DefaultChipWidth / 2, SharedConstants.DefaultChipHeight / 3)]
        public void TestToMLMultiArray_SmallThanDefault(int width, int height)
        {
            var chip = GraphicUtils.GetChipByRegion(0, 0, width, height);
            var result = chip.ToMLMultiArray();

            Assert.AreEqual(new nint[] {1, 1, 3, SharedConstants.DefaultChipWidth, SharedConstants.DefaultChipHeight},
                result.Shape);

            for (var i = 0; i < SharedConstants.DefaultChipWidth; i++)
            {
                for (var j = 0; j < SharedConstants.DefaultChipHeight; j++)
                {
                    if (i < chip.Region.Width && j < chip.Region.Height)
                    {
                        AssertMLMultiArrayEntry(chip.Pixels[i][j], result, i, j);
                    }
                    else
                    {
                        AssertMLMultiArrayEntry(SKColors.Black, result, i, j);
                    }
                }
            }
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, SharedConstants.DefaultChipHeight / 2, SharedConstants.DefaultChipWidth / 3)]
        [TestCase(2, SharedConstants.DefaultChipHeight - 1, SharedConstants.DefaultChipWidth - 1)]
        public void TestComputeIndex(int i, int j, int k)
        {
            Assert.AreEqual(
                i * SharedConstants.DefaultChipHeight * SharedConstants.DefaultChipWidth +
                j * SharedConstants.DefaultChipWidth + k, PreProcessingService.ComputeIndex(i, j, k));
        }

        /// <summary>
        /// Checks if the specified <see cref="MLMultiArray"/> has correct values at (<paramref name="i"/>,
        /// <paramref name="j"/>) of the chip.
        /// </summary>
        /// <param name="pixel">The specified 2D array of <see cref="SKColor"/>s.</param>
        /// <param name="result">The specified <see cref="MLMultiArray"/>.</param>
        /// <param name="i">The specified index i.</param>
        /// <param name="j">The specified index j.</param>
        private static void AssertMLMultiArrayEntry(SKColor pixel, MLMultiArray result, int i, int j)
        {
            var h = SharedConstants.DefaultChipHeight - j - 1;
            var w = SharedConstants.DefaultChipWidth - i - 1;
            Assert.AreEqual(ColorMapping[pixel.Red].DoubleValue,
                result[PreProcessingService.ComputeIndex(0, h, w)].DoubleValue, Tolerance);
            Assert.AreEqual(ColorMapping[pixel.Green].DoubleValue,
                result[PreProcessingService.ComputeIndex(1, h, w)].DoubleValue, Tolerance);
            Assert.AreEqual(ColorMapping[pixel.Blue].DoubleValue,
                result[PreProcessingService.ComputeIndex(2, h, w)].DoubleValue, Tolerance);
        }
    }
}
