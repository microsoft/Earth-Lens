using EarthLens.ClassExtensions;
using EarthLens.Tests.TestUtils;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.ClassExtensions
{
    [TestFixture]
    public class SKRectExtensionsTests
    {
        private const float Tolerance = 1e-3f;

        private SKRect _rect1;
        private SKRect _rect2;

        [SetUp]
        public void SetUp()
        {
            _rect1 = new SKRect(0.1f, 0.2f, 0.3f, 0.4f);
            _rect2 = _rect1.Scale(new SKSize(500f, 600f));
        }

        [Test]
        public void TestScale()
        {
            var factor = new SKSize(500, 600);
            var result = _rect1.Scale(factor);
            AssertUtils.AreEqualSKRect(new SKRect(0.1f * 500, 0.2f * 600, 0.3f * 500, 0.4f * 600), result);
        }

        [Test]
        public void TestTranslate()
        {
            var offset = new SKPoint(5.6f, 7.8f);
            var result = _rect1.Translate(offset);
            AssertUtils.AreEqualSKRect(new SKRect(0.1f + 5.6f, 0.2f + 7.8f, 0.3f + 5.6f, 0.4f + 7.8f), result);
        }

        [Test]
        public void TestGetArea()
        {
            var result = _rect2.GetArea();
            Assert.AreEqual(.2f * 500 * .2f * 600, result, Tolerance);
        }

        [Test]
        public void TestIntersection_NotIntersect()
        {
            var rect1 = new SKRect(1f, 2f, 3f, 4f);
            var rect2 = new SKRect(3f, 4f, 5f, 7f);
            var result = SKRectExtensions.Intersection(rect1, rect2);
            Assert.AreEqual(0.0f, result, Tolerance);
        }

        [Test]
        public void TestIntersection_Intersect()
        {
            var rect1 = new SKRect(1f, 2f, 3f, 4f);
            var rect2 = new SKRect(2f, 3f, 5f, 7f);
            var result = SKRectExtensions.Intersection(rect1, rect2);
            Assert.AreEqual(1.0f, result, Tolerance);
        }

        [Test]
        public void TestUnion_NotIntersect()
        {
            var rect1 = new SKRect(1f, 2f, 3f, 4f);
            var rect2 = new SKRect(3f, 4f, 5f, 7f);
            var result = SKRectExtensions.Union(rect1, rect2);
            Assert.AreEqual(10.0f, result, Tolerance);
        }

        [Test]
        public void TestUnion_Intersect()
        {
            var rect1 = new SKRect(1f, 2f, 3f, 4f);
            var rect2 = new SKRect(2f, 3f, 5f, 7f);
            var result = SKRectExtensions.Union(rect1, rect2);
            Assert.AreEqual(15.0f, result, Tolerance);
        }

        [Test]
        public void TestIntersectionOverUnion()
        {
            var rect1 = new SKRect(1f, 2f, 3f, 4f);
            var rect2 = new SKRect(2f, 3f, 5f, 7f);
            var result = SKRectExtensions.IntersectionOverUnion(rect1, rect2);
            Assert.AreEqual(1.0f / 15.0f, result);
        }
    }
}
