using System;
using System.Collections.Generic;

namespace EarthLens.Models
{
    public class GalleryImageSection : SortedList<ImageEntry, ImageEntry>
    {
        public DateTime CreationDate { get; set; }

        public GalleryImageSection(DateTime creationDate) : base(new ImageEntryComparer())
        {
            CreationDate = creationDate;
        }

        public void Add(ImageEntry imageEntry)
        {
            Add(imageEntry, imageEntry);
        }
    }
}