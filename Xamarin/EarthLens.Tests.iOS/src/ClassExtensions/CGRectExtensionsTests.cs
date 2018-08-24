using EarthLens.iOS.ClassExtensions;
using EarthLens.Tests.iOS.TestUtils;
using EarthLens.Tests.TestUtils;
using CoreGraphics;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.iOS.ClassExtensions
{
    [TestFixture]
    public class CGRectExtensionsTests
    {
        private CGRect _rect;

        [SetUp]
        public void SetUp()
        {
            _rect = new CGRect(0.1, 0.2, 0.3, 0.4);
        }

        [Test]
        public void TestScale()
        {
            var factor = new CGSize(500, 600);
            var result = _rect.Scale(factor);
            AssertUtilsIOS.AreEqualCGRect(
                new CGRect(_rect.X * factor.Width, _rect.Y * factor.Height, _rect.Width * factor.Width,
                    _rect.Height * factor.Height), result);
        }

        [Test]
        public void TestTranslate()
        {
            var offset = new CGPoint(5.6, 7.8);
            var result = _rect.Translate(offset);
            AssertUtilsIOS.AreEqualCGRect(new CGRect(_rect.X + offset.X, _rect.Y + offset.Y, _rect.Width, _rect.Height),
                result);
        }

        [Test]
        public void TestNormalize()
        {
            var rect = new CGRect(500, 600, 700, 800);
            var size = new CGSize(900, 1000);
            var result = rect.Normalize(size);
            AssertUtilsIOS.AreEqualCGRect(
                new CGRect(rect.X / size.Width, rect.Y / size.Height, rect.Width / size.Width,
                    rect.Height / size.Height), result);
        }

        [Test]
        public void TestToSKRect()
        {
            var result = _rect.ToSKRect();
            AssertUtils.AreEqualSKRect(
                new SKRect((float) _rect.X, (float) _rect.Y, (float) (_rect.X + _rect.Width),
                    (float) (_rect.Y + _rect.Height)), result);
        }
    }
}
