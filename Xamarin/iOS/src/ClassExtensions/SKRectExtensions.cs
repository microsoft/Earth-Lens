using CoreGraphics;
using SkiaSharp;

namespace EarthLens.iOS.ClassExtensions
{
    public static class SKRectExtensions
    {
        /// <summary>
        /// Transforms the specified <see cref="SKRect"/> to the corresponding <see cref="CGRect"/> object.
        /// </summary>
        /// <returns>The <see cref="CGRect"/> after transformation.</returns>
        /// <param name="self">The <see cref="SKRect"/> to transform.</param>
        public static CGRect ToCGRect(this SKRect self) => new CGRect(
            self.MidX - (self.Width / 2),
            self.MidY - (self.Height / 2),
            self.Width,
            self.Height);
    }
}
