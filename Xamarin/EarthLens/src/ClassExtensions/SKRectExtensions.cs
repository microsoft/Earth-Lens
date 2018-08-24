using SkiaSharp;

namespace EarthLens.ClassExtensions
{
    public static class SKRectExtensions
    {
        /// <summary>
        /// Scales this <see cref="SKRect"/> coordinates by the specified factors.
        /// </summary>
        /// <returns>The coordinates after scaling.</returns>
        /// <param name="self">The coordinates to scale.</param>
        /// <param name="factor">The width and height factors to scale by.</param>
        public static SKRect Scale(this SKRect self, SKSize factor) => new SKRect(
            self.Left * factor.Width,
            self.Top * factor.Height,
            self.Right * factor.Width,
            self.Bottom * factor.Height);

        /// <summary>
        /// Translates this <see cref="SKRect"/> by the specified offset.
        /// </summary>
        /// <returns>The <see cref="SKRect"/> after translation.</returns>
        /// <param name="self">The <see cref="SKRect"/> to translate.</param>
        /// <param name="offset">The offset to translate by.</param>
        public static SKRect Translate(this SKRect self, SKPoint offset) => new SKRect(
            self.Left + offset.X,
            self.Top + offset.Y,
            self.Right + offset.X,
            self.Bottom + offset.Y);

        /// <summary>
        /// Returns the IOU (intersection over union) of the two specified <see cref="SKRect"/>.
        /// </summary>
        /// <returns>The IOU of the two specified <see cref="SKRect"/>.</returns>
        /// <param name="rect1">The first <see cref="SKRect"/>.</param>
        /// <param name="rect2">The second <see cref="SKRect"/>.</param>
        public static float IntersectionOverUnion(SKRect rect1, SKRect rect2)
        {
            float union = Union(rect1, rect2);
            float intersection = Intersection(rect1, rect2);
            return intersection / union;
        }

        /// <summary>
        /// Returns the area of the intersection of the two specified <see cref="SKRect"/>.
        /// </summary>
        /// <returns>The area of the intersection.</returns>
        /// <param name="rect1">The first <see cref="SKRect"/>.</param>
        /// <param name="rect2">The second <see cref="SKRect"/>.</param>
        public static float Intersection(SKRect rect1, SKRect rect2)
        {
            var rect = new SKRect(rect1.Left, rect1.Top, rect1.Right, rect1.Bottom);
            rect.Intersect(rect2);
            return rect.GetArea();
        }

        /// <summary>
        /// Returns the area of the union of the two specified <see cref="SKRect"/>.
        /// </summary>
        /// <returns>The area of the union.</returns>
        /// <param name="rect1">The first <see cref="SKRect"/>.</param>
        /// <param name="rect2">The second <see cref="SKRect"/>.</param>
        public static float Union(SKRect rect1, SKRect rect2)
        {
            return rect1.GetArea() + rect2.GetArea() - Intersection(rect1, rect2);
        }

        /// <summary>
        /// Returns the area of the specified <see cref="SKRect"/>.
        /// </summary>
        /// <returns>The area of the specified <see cref="SKRect"/>.</returns>
        /// <param name="rect">The <see cref="SKRect"/>.</param>
        public static float GetArea(this SKRect rect)
        {
            return rect.Width * rect.Height;
        }
    }
}
