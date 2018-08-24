using System;
using EarthLens.ClassExtensions;
using SkiaSharp;

namespace EarthLens.Models
{
    public class Observation
    {
        public Category Category { get; private set; }
        public SKRect BoundingBox { get; private set; }
        public double Confidence { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Observation"/> class, and adds it to the specified <see cref="Category"/>.
        /// </summary>
        /// <param name="category">The category of the <see cref="Observation"/>.</param>
        /// <param name="confidence">The confidence score of the <see cref="Observation"/> given by the model.</param>
        /// <param name="boundingBox">The bounding box rectangle of the <see cref="Observation"/>.</param>
        public Observation(Category category, double confidence, SKRect boundingBox)
        {
            Category = category ?? throw new ArgumentNullException();
            Confidence = confidence;
            BoundingBox = boundingBox;
            category.Add(this);
        }

        /// <summary>
        /// Scales this <see cref="Observation"/> by the specified size and translates it by the specified offset.
        /// </summary>
        /// <param name="size">The size of the corresponding <see cref="Chip"/>.</param>
        /// <param name="offset">The offset of the corresponding <see cref="Chip"/>.</param>
        public void ScaleAndTranslate(SKSizeI size, SKPointI offset)
        {
            var result = BoundingBox.Scale(size)
                                    .Translate(offset);
            BoundingBox = result;
        }

        /// <summary>
        /// Resets the <see cref="Category"/> of an <see cref="Observation"/>  to a <param name="newCategory"/> ,
        /// and puts the observation under the <param name="newCategory">.
        /// Should be used only by a View Controller when reassigning categories to observations.
        /// </summary>
        /// <param name="newCategory"> the newCategory the observation should be assigned to.</param>
        public void ResetCategory(Category newCategory)
        {
            if (newCategory == null)
            {
                throw new ArgumentNullException();
            }

            var originalCategory = Category;

            // remove this observation from current Category 
            originalCategory.Remove(this);
            Category = newCategory;

            // Add this observation to the new category
            newCategory.Add(this);

            // assign bounding box colour
            Category.Color = newCategory.Color;
        }

        protected bool Equals(Observation other)
        {
            return Equals(Category, other.Category) && Confidence.Equals(other.Confidence) &&
                   BoundingBox.Equals(other.BoundingBox);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Observation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Confidence.GetHashCode();
                hashCode = (hashCode * 397) ^ BoundingBox.GetHashCode();
                return hashCode;
            }
        }
    }
}
