using EarthLens.Models;
using EarthLens.Tests.TestUtils;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.Models
{
    [TestFixture]
    public class ObservationTests
    {
        private Category _category;
        private Category _category2;
        private double _confidence;
        private double _confidence2;
        private SKRect _rect;
        private Observation _observation;
        private Observation _observation2;

        [SetUp]
        public void SetUp()
        {
            _category = (new CategoryManager()).GetOrCreate(18, "Small Car");
            _confidence = 0.314159;
            _confidence2 = 0.254;
            _rect = new SKRect(0.1f, 0.2f, 0.3f, 0.4f);
            _observation = new Observation(_category, _confidence, _rect);
            _observation2 = new Observation(_category, _confidence2, _rect);
            _category2 = new Category(1, "Small Rabbit");
        }

        [Test]
        public void TestConstructor()
        {
            Assert.AreSame(_category, _observation.Category);
            Assert.AreEqual(_confidence, _observation.Confidence);
            Assert.AreEqual(_rect, _observation.BoundingBox);
        }

        [TestCase(0, 0)]
        [TestCase(300, 600)]
        public void TestScaleAndTranslate(int offsetX, int offsetY)
        {
            var offset = new SKPointI(offsetX, offsetY);
            _observation.ScaleAndTranslate(
                new SKSizeI(SharedConstants.DefaultChipWidth, SharedConstants.DefaultChipHeight), offset);
            var expectedBoundingBox = new SKRect(
                _rect.Left * SharedConstants.DefaultChipWidth + offsetX,
                _rect.Top * SharedConstants.DefaultChipHeight + offsetY,
                _rect.Right * SharedConstants.DefaultChipWidth + offsetX,
                _rect.Bottom * SharedConstants.DefaultChipHeight + offsetY);

            Assert.AreEqual(_category, _observation.Category);
            Assert.AreEqual(_confidence, _observation.Confidence);
            AssertUtils.AreEqualSKRect(expectedBoundingBox, _observation.BoundingBox);
        }

        [TestCase]
        public void TestResetCategory()
        {
            Assert.AreEqual(2, _category.Observations.Count);

            _observation2.ResetCategory(_category2);

            Assert.AreEqual(1, _category2.Observations.Count); 
            Assert.AreEqual(1, _category.Observations.Count);
        }
    }
}
