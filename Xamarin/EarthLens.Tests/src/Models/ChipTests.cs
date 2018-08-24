using System.Linq;
using EarthLens.Models;
using EarthLens.Tests.TestUtils;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.Models
{
    [TestFixture]
    public class ChipTests
    {
        [Test]
        public void TestConstructor()
        {
            var chip = GraphicUtils.GetChipByRegion(10, 20, 40, 50);
            var expectedRegion = new SKRectI(10, 20, 50, 70);
            var expectedPixels = GraphicUtils.GetPixelsByDimensions(40, 50);

            Assert.AreEqual(expectedRegion, chip.Region);
            Assert.AreEqual(expectedPixels, chip.Pixels);
        }

        [TestCase(3, 4)]
        public void TestFromImage_DimensionsDivisibleByDefault(int widthMultiplier, int heightMultiplier)
        {
            var image = GraphicUtils.GetImageByDimensions(widthMultiplier * SharedConstants.DefaultChipWidth,
                heightMultiplier * SharedConstants.DefaultChipHeight);
            var chips = Chip.FromImage(image).ToList();

            Assert.AreEqual(widthMultiplier * heightMultiplier, chips.Count);

            for (var i = 0; i < widthMultiplier; i++)
            {
                for (var j = 0; j < heightMultiplier; j++)
                {
                    var left = i * SharedConstants.DefaultChipWidth;
                    var top = j * SharedConstants.DefaultChipHeight;
                    var expectedRegion = new SKRectI(left, top, left + SharedConstants.DefaultChipWidth,
                        top + SharedConstants.DefaultChipHeight);
                    var expectedPixels = GraphicUtils.GetPixelsByDimensions(SharedConstants.DefaultChipWidth,
                        SharedConstants.DefaultChipHeight);
                    Assert.AreEqual(expectedRegion, chips[i * heightMultiplier + j].Region);
                    Assert.AreEqual(expectedPixels, chips[i * heightMultiplier + j].Pixels);
                }
            }
        }

        [TestCase(SharedConstants.DefaultChipWidth / 2, SharedConstants.DefaultChipHeight / 3)]
        public void TestFromImage_DimensionsLessThanDefault(int width, int height)
        {
            var image = GraphicUtils.GetImageByDimensions(width, height);
            var chips = Chip.FromImage(image).ToList();

            Assert.AreEqual(1, chips.Count);

            var expectedRegion = new SKRectI(0, 0, width, height);
            var expectedPixels = GraphicUtils.GetPixelsByDimensions(width, height);
            Assert.AreEqual(expectedRegion, chips[0].Region);
            Assert.AreEqual(expectedPixels, chips[0].Pixels);
        }

        [TestCase(SharedConstants.DefaultChipWidth * 2 + SharedConstants.DefaultChipWidth / 2, SharedConstants.DefaultChipHeight * 3 + SharedConstants.DefaultChipHeight / 3)]
        public void TestFromImage_DimensionsNotDivisibleByDefault(int width, int height)
        {
            var image = GraphicUtils.GetImageByDimensions(width, height);
            var chips = Chip.FromImage(image).ToList();

            var numWidth = width / SharedConstants.DefaultChipWidth + 1;
            var numHeight = height / SharedConstants.DefaultChipHeight + 1;

            for (var i = 0; i < numWidth; i++)
            {
                var subWidth = i + 1 < numWidth
                    ? SharedConstants.DefaultChipWidth
                    : width % SharedConstants.DefaultChipWidth;

                for (var j = 0; j < numHeight; j++)
                {
                    var subHeight = j + 1 < numHeight
                        ? SharedConstants.DefaultChipHeight
                        : height % SharedConstants.DefaultChipHeight;

                    var left = i * SharedConstants.DefaultChipWidth;
                    var top = j * SharedConstants.DefaultChipHeight;
                    var expectedRegion = new SKRectI(left, top, left + subWidth, top + subHeight);
                    var expectedPixels = GraphicUtils.GetPixelsByDimensions(subWidth, subHeight);
                    Assert.AreEqual(expectedRegion, chips[i * numHeight + j].Region);
                    Assert.AreEqual(expectedPixels, chips[i * numHeight + j].Pixels);
                }
            }
        }
    }
}
