using System;
using System.Collections.Generic;
using EarthLens.Models;
using EarthLens.Services;
using Foundation;

namespace EarthLens.iOS.Gallery
{
    public static class GalleryDataSource
    {
        public static SortedList<DateTime, GalleryImageSection> ImageCollection { get; set; }
        public static IList<Observation> Observations { get; }
        public static ISet<NSIndexPath> SelectedImageIndexPaths { get; }
        public static int NumberOfSelectedImages { get; set; }
        
        static GalleryDataSource()
        {
            ImageCollection = NSUserDefaults.StandardUserDefaults.BoolForKey(Constants.GallerySettingsSwitchState)
                ? new SortedList<DateTime, GalleryImageSection>()
                : new SortedList<DateTime, GalleryImageSection>(new ReverseDateTimeComparer());
            Observations = new List<Observation>();
            SelectedImageIndexPaths = new HashSet<NSIndexPath>();
        }

        public static void LoadImages()
        {
            var retrievedImages = DatabaseService.GetAllImages();
            ImageCollection.Clear();

            foreach (var dao in retrievedImages)
            {
                var img = ImageEncodingService.Base64ToSKImage(dao.Base64);
                var galleryImageEntry = new ImageEntry(img, dao.Name, dao.CreationTime, dao.Id);
                if (ImageCollection.ContainsKey(dao.CreationTime.Date))
                {
                    ImageCollection[dao.CreationTime.Date].Add(galleryImageEntry);
                }
                else
                {
                    ImageCollection.Add(dao.CreationTime.Date, new GalleryImageSection(dao.CreationTime.Date));
                    ImageCollection[dao.CreationTime.Date].Add(galleryImageEntry);
                }
            }
        }
    }
}