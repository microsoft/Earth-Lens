using System;
using System.Collections.Generic;
using System.Linq;
using EarthLens.Models;
using CoreML;
using Foundation;

namespace EarthLens.iOS.Services
{
    public static class PreProcessingService
    {
        public static readonly IList<NSNumber> ColorMapping = PreComputeColorMapping().ToList().AsReadOnly();

        /// <summary>
        /// Converts the pixels from the specified <see cref="Chip"/> to a <see cref="MLMultiArray"/>.
        /// </summary>
        /// <returns>The <see cref="MLMultiArray"/> corresponding to the pixels.</returns>
        /// <param name="chip">The <see cref="Chip"/> to convert.</param>
        public static MLMultiArray ToMLMultiArray(this Chip chip)
        {
            if (chip == null)
            {
                throw new ArgumentNullException();
            }

            // mlMultiArray is transposed with respect to the diagonal
            var mlMultiArray =
                new MLMultiArray(
                    new NSNumber[]
                    {
                        Constants.SequenceLength,
                        Constants.BatchSize,
                        Constants.ImageChannelSize,
                        SharedConstants.DefaultChipHeight,
                        SharedConstants.DefaultChipWidth
                    },
                    MLMultiArrayDataType.Double,
                    out _);

            // Currently we assume Constants.SequenceLength == 1 and Constants.BatchSize == 1.
            // Once we support sequence and batch input, we should refactor this and the ComputeIndex method.
            for (var i = 0; i < SharedConstants.DefaultChipWidth; i++)
            {
                for (var j = 0; j < SharedConstants.DefaultChipHeight; j++)
                {
                    var h = SharedConstants.DefaultChipHeight - j - 1;
                    var w = SharedConstants.DefaultChipWidth - i - 1;
                    if (i < chip.Region.Width && j < chip.Region.Height)
                    {
                        mlMultiArray[ComputeIndex(0, h, w)] = ColorMapping[chip.Pixels[i][j].Red];
                        mlMultiArray[ComputeIndex(1, h, w)] = ColorMapping[chip.Pixels[i][j].Green];
                        mlMultiArray[ComputeIndex(2, h, w)] = ColorMapping[chip.Pixels[i][j].Blue];
                    }
                    else
                    {
                        mlMultiArray[ComputeIndex(0, h, w)] = ColorMapping[0];
                        mlMultiArray[ComputeIndex(1, h, w)] = ColorMapping[0];
                        mlMultiArray[ComputeIndex(2, h, w)] = ColorMapping[0];
                    }
                }
            }

            return mlMultiArray;
        }
        
        /// <summary>
        /// Converts the specified 3-D index (<paramref name="rgbChannel"/>, <paramref name="yCoordinate"/>,
        /// <paramref name="xCoordinate"/>) to the corresponding 1-D index.
        /// </summary>
        /// <param name="rgbChannel">The first dimension of the specified 3-D index (RGB channel).</param>
        /// <param name="yCoordinate">The second dimension of the specified 3-D index (y-coordinate of the chip).</param>
        /// <param name="xCoordinate">The third dimension of the specified 3-D index (x-coordinate of the chip).</param>
        /// <returns>The corresponding 1-D index.</returns>
        public static int ComputeIndex(int rgbChannel, int yCoordinate, int xCoordinate)
        {
            checked
            {
                return rgbChannel * SharedConstants.DefaultChipWidth * SharedConstants.DefaultChipHeight +
                       yCoordinate * SharedConstants.DefaultChipWidth + xCoordinate;
            }
        }

        /// <summary>
        /// Normalizes the specified color in the range of 0 .. 255 to the range of -1 .. 1.
        /// </summary>
        /// <param name="color">The specified color in the range of 0 .. 255.</param>
        /// <returns>The normalized color in the range of -1 .. 1.</returns>
        private static NSNumber NormalizeColor(int color)
        {
            return new NSNumber(color * 2.0 / byte.MaxValue - 1);
        }

        /// <summary>
        /// Pre-compute all color mappings from range of 0..255 to range of -1..1.
        /// The pre-computation is necessary because the constructor of <see cref="NSNumber"/> is slow (~ 3 seconds for
        /// 3 * <see cref="SharedConstants.DefaultChipHeight"/> * <see cref="SharedConstants.DefaultChipWidth"/> calls).
        /// </summary>
        /// <returns>An <see cref="IEnumerable{NSNumber}"/> consisting of all color mappings.</returns>
        private static IEnumerable<NSNumber> PreComputeColorMapping()
        {
            var mapping = new NSNumber[byte.MaxValue + 1];

            for (var i = (int)byte.MinValue; i <= byte.MaxValue; i++)
            {
                mapping[i] = NormalizeColor(i);
            }

            return mapping;
        }
    }
}
