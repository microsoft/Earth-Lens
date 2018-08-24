using EarthLens.iOS.ClassExtensions;
using EarthLens.Tests.iOS.TestUtils;
using CoreGraphics;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.iOS.ClassExtensions
{
    [TestFixture]
    public class SKRectExtensionsTests
    {
        private SKRect _rect;

        [SetUp]
        public void SetUp()
        {
            _rect = new SKRect(0.1f, 0.1f, 1.2f, 1.2f);
        }

        [Test]
        public void TestToCGRect()
        {
            var result = _rect.ToCGRect();
            AssertUtilsIOS.AreEqualCGRect(new CGRect(0.1, 0.1, 1.1, 1.1), result);
        }
    }
}
