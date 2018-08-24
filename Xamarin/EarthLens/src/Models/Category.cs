using System;
using System.Collections.Generic;
using SkiaSharp;

namespace EarthLens.Models
{
    public class Category : IEquatable<Category>
    {
        public int Id { get; }
        public string Label { get; }
        public HashSet<Observation> Observations { get; }
        public SKColor Color { get; set; }

        public MegaCategory MegaCategory => MegaCategory.GetMegaCategoryMapping(this);

        /// <summary>
        /// Initializes a <see cref="Category"/> instance with the specified ID and label.
        /// Should be used only by <see cref="CategoryManager"/>.
        /// </summary>
        /// <param name="id">The ID of this <see cref="Category"/>.</param>
        /// <param name="label">The label of this <see cref="Category"/>.</param>
        internal Category(int id, string label)
        {
            Id = id;
            Label = label;
            Observations = new HashSet<Observation>();

            var numberOfColors = SharedConstants.AllAvailableColors.Count;
            var hash = (label.GetHashCode() % numberOfColors + numberOfColors) % numberOfColors;
            Color = SharedConstants.AllAvailableColors[(id > 0 ? id - 1 : hash) % numberOfColors];
        }

        /// <summary>
        /// Adds the specified <see cref="Observation"/> to this <see cref="Category"/>.
        /// Should be used only by <see cref="Observation(Category, double, SkiaSharp.SKRect)"/>.
        /// </summary>
        /// <param name="observation">The <see cref="Observation"/> to add.</param>
        internal void Add(Observation observation)
        {
            Observations.Add(observation);
        }

        /// <summary>
        /// Remove the specified <see cref="Observation"/> to this <see cref="Category"/>.
        /// Should be used only by <see cref="Observation.ResetCategory"/> in the <see cref="Observation"/> class.
        /// </summary>
        /// <param name="observation">The <see cref="Observation"/> to delete.</param>
        internal void Remove(Observation observation)
        {
            Observations.Remove(observation);
        }

        public bool Equals(Category other)
        {
            return Id == other.Id && string.Equals(Label, other.Label);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Category) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Label != null ? Label.GetHashCode() : 0);
            }
        }
    }
}