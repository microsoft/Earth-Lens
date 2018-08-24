using System;
using System.Linq;
using EarthLens.Models;
using EarthLens.Tests.TestUtils;
using NUnit.Framework;
using EarthLens.Services;
using SkiaSharp;

namespace EarthLens.Tests.Services
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        [SetUp]
        public void Setup()
        {
            DatabaseService.DropTables();
            DatabaseService.CreateTables();
        }

        [Test]
        public void TestAddImage()
        {
            var image = GraphicUtils.GetImageByDimensions(100, 100);
            var category = new CategoryManager().GetOrCreate(18, "Small Car");
            const string name = "Image.jpg";
            var creationTime = DateTime.UtcNow;
            var observations = new Observation[]
            {
                new Observation(category, 0.9, new SKRect(image.Width / 5, image.Height / 4, image.Width / 3, image.Height / 2)),
                new Observation(category, 0.9, new SKRect(image.Width / 6, image.Height / 7, image.Width * 2 / 3, image.Height * 3 / 4))
            };

            var addedImageDao = DatabaseService.InsertOrUpdate(new ImageEntry(image, name, creationTime, null, observations));
            var retrievedImageDao = DatabaseService.GetImage(addedImageDao.Id);

            Assert.AreEqual(addedImageDao, retrievedImageDao);
        }

        [Test]
        public void TestRemoveImage()
        {
            var image = GraphicUtils.GetImageByDimensions(100, 100);
            var category = new CategoryManager().GetOrCreate(18, "Small Car");
            const string name = "Image.jpg";
            var creationTime = DateTime.UtcNow;
            var observations = new Observation[]
            {
                new Observation(category, 0.9, new SKRect(image.Width / 5, image.Height / 4, image.Width / 3, image.Height / 2)),
                new Observation(category, 0.9, new SKRect(image.Width / 6, image.Height / 7, image.Width * 2 / 3, image.Height * 3 / 4))
            };
            
            var imageDaoToDelete = DatabaseService.InsertOrUpdate(new ImageEntry(image, name, creationTime, null, observations));
            DatabaseService.DeleteImage(imageDaoToDelete.Id);
            
            Assert.IsNull(DatabaseService.GetImage(imageDaoToDelete.Id));
            Assert.IsEmpty(DatabaseUtils.GetAllObservations());
        }

        [Test]
        public void TestAddAndRemoveMultipleImages()
        {
            var image1 = GraphicUtils.GetImageByDimensions(100, 100);
            var image2 = GraphicUtils.GetImageByDimensions(200, 200);
            
            var category1 = new CategoryManager().GetOrCreate(18, "Small Car");
            var observations1 = new Observation[]
            {
                new Observation(category1, 0.9, new SKRect(image1.Width / 5, image1.Height / 4, image1.Width / 3, image1.Height / 2)),
                new Observation(category1, 0.9, new SKRect(image1.Width / 6, image1.Height / 7, image1.Width * 2 / 3, image1.Height * 3 / 4))
            };
            
            var category2 = new CategoryManager().GetOrCreate(19, "Large Boat");
            var observations2 = new[]
            {
                new Observation(category2, 0.9, new SKRect(image2.Width / 5, image2.Height / 4, image2.Width / 3, image2.Height / 2))
            };

            const string name1 = "Image1.jpg";
            const string name2 = "Image2.jpg";
            var creationTime1 = DateTime.UtcNow;
            var creationTime2 = DateTime.UtcNow;

            var imageDao1 = DatabaseService.InsertOrUpdate(new ImageEntry(image1, name1, creationTime1, null, observations1));
            var imageDao2 = DatabaseService.InsertOrUpdate(new ImageEntry(image2, name2, creationTime2, null, observations2));
            
            DatabaseService.DeleteImage(imageDao1.Id);
            
            Assert.IsNull(DatabaseService.GetImage(imageDao1.Id));
            
            CollectionAssert.AreEqual(imageDao2.Observations, DatabaseUtils.GetAllObservations().ToList());      
        }      
    }
}
