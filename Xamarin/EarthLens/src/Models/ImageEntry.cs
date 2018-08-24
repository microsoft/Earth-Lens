using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace EarthLens.Models
{
    public class ImageEntry
    {
        public SKImage Image { get; }
        public string Name { get; }
        public DateTime? CreationTime { get; }
        public int? Id { get; set; }
        public IList<Observation> Observations { get; set; }

        /// <summary>
        /// Constructs an <see cref="ImageEntry"/> with the specified <see cref="SKImage"/>, creation time, ID and
        /// <see cref="Observation"/>s.
        /// </summary>
        /// <param name="image">The specified <see cref="SKImage"/>.</param>
        /// <param name="name">The original image filename.</param>
        /// <param name="creationTime">The specified creation time.</param>
        /// <param name="id">The specified, optional ID.</param>
        /// <param name="observations">The specified <see cref="Observation"/>s associated with the image.</param>
        public ImageEntry(SKImage image, string name, DateTime? creationTime, int? id, IEnumerable<Observation>
            observations)
        {
            Image = image;
            Name = name;
            CreationTime = creationTime;
            Id = id;
            Observations = observations.ToList();
        }

        /// <summary>
        /// Constructs an <see cref="ImageEntry"/> with the specified <see cref="SKImage"/>, creation time, ID and
        /// no <see cref="Observation"/>s.
        /// </summary>
        /// <param name="image">The specified <see cref="SKImage"/>.</param>
        /// <param name="name">The original image filename.</param>
        /// <param name="creationTime">The specified creation time.</param>
        /// <param name="id">The specified, optional ID.</param>
        public ImageEntry(SKImage image, string name, DateTime? creationTime, int? id)
        {
            Image = image;
            Name = name;
            CreationTime = creationTime;
            Id = id;
            Observations = new List<Observation>();
        }
    }

    public class ReverseDateTimeComparer : IComparer<DateTime>
    {
        public int Compare(DateTime x, DateTime y)
        {
            return DateTime.Compare(y, x);
        }
    }

    public class ImageEntryComparer : IComparer<ImageEntry>
    {
        public int Compare(ImageEntry x, ImageEntry y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException();
            }

            var dateTimeComparison =
                DateTime.Compare(x.CreationTime ?? DateTime.MaxValue, y.CreationTime ?? DateTime.MaxValue);
            return dateTimeComparison != 0 ? dateTimeComparison : Comparer<int?>.Default.Compare(x.Id, y.Id);
        }
    }
}