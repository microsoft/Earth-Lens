using CoreGraphics;
using NUnit.Framework;

namespace EarthLens.Tests.iOS.TestUtils
{
    public static class AssertUtilsIOS
    {
        private const double Tolerance = 1e-6;
        
        /// <summary>
        /// Checks if the two specified <see cref="CGRect"/> are equal within the tolerance.
        /// </summary>
        /// <param name="expected">The expected <see cref="CGRect"/>.</param>
        /// <param name="actual">The actual <see cref="CGRect"/>.</param>
        public static void AreEqualCGRect(CGRect expected, CGRect actual)
        {
            Assert.AreEqual(expected.X, actual.X, Tolerance);
            Assert.AreEqual(expected.Y, actual.Y, Tolerance);
            Assert.AreEqual(expected.Width, actual.Width, Tolerance);
            Assert.AreEqual(expected.Height, actual.Height, Tolerance);
        }
    }
}