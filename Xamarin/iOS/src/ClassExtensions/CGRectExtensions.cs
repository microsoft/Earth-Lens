using CoreGraphics;
using SkiaSharp;

namespace EarthLens.iOS.ClassExtensions
{
    public static class CGRectExtensions
    {
        /// <summary>
        /// Scales this <see cref="CGRect"/> coordinates by the specified factors.
        /// </summary>
        /// <returns>The coordinates after scaling.</returns>
        /// <param name="self">The coordinates to scale.</param>
        /// <param name="factor">The width and height factors to scale by.</param>
        public static CGRect Scale(this CGRect self, CGSize factor) => new CGRect(
            self.X * factor.Width,
            self.Y * factor.Height,
            self.Size.Width * factor.Width,
            self.Size.Height * factor.Height);

        /// <summary>
        /// Translates this <see cref="CGRect"/> by the specified offset.
        /// </summary>
        /// <returns>The <see cref="CGRect"/> after translation.</returns>
        /// <param name="self">The <see cref="CGRect"/> to translate.</param>
        /// <param name="offset">The offset to translate by.</param>
        public static CGRect Translate(this CGRect self, CGPoint offset) => new CGRect(
            self.X + offset.X,
            self.Y + offset.Y,
            self.Width,
            self.Height);

        /// <summary>
        /// Normalize the specified <see cref="CGRect"/> to the specified size.
        /// </summary>
        /// <returns>The normalized <see cref="CGRect"/>.</returns>
        /// <param name="self">The <see cref="CGRect"/> to normalize.</param>
        /// <param name="size">Size.</param>
        public static CGRect Normalize(this CGRect self, CGSize size) => new CGRect(
            self.X / size.Width,
            self.Y / size.Height,
            self.Width / size.Width,
            self.Height / size.Height);

        /// <summary>
        /// Transforms the specified <see cref="CGRect"/> to the corresponding <see cref="SKRect"/> object.
        /// </summary>
        /// <returns>The <see cref="SKRect"/> after transformation.</returns>
        /// <param name="self">The <see cref="CGRect"/> to transform.</param>
        public static SKRect ToSKRect(this CGRect self) => new SKRect(
            (float)self.X,
            (float)self.Y,
            (float)(self.X + self.Width),
            (float)(self.Y + self.Height));
    }
}
