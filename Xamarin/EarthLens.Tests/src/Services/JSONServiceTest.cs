using System;
using System.Collections.Generic;
using EarthLens.Tests.TestUtils;
using EarthLens.ORM;
using NUnit.Framework;
using Newtonsoft.Json;
using EarthLens.Models;
using SkiaSharp;
using EarthLens.Services;

namespace EarthLens.Tests.Services
{
    [TestFixture]
    public class JSONServiceTest
    {
        private const double Epislon = 1e-7;
        private CategoryManager _manager;

        [SetUp]
        public void InitializeCategoryManager()
        {
            _manager = new CategoryManager();
        }

        private (ImageDAO, string) GenerateJsonsExamples()
        {
            var observations = new List<Observation>()
            {
                new Observation( _manager.GetOrCreate(0,"Car"), 0.5, new SKRect(0.1f,0.2f,0.7f,0.8f)),
                new Observation( _manager.GetOrCreate(1,"Bat Mobile"), 0.7, new SKRect(0.1f,0.2f,0.7f,0.8f)),
            };

            const string name = "Image.jpg";
            var creationTime = DateTime.UtcNow;
            var testImg =
                GraphicUtils.GetImageByDimensions(SharedConstants.DefaultChipWidth, SharedConstants.DefaultChipHeight);
            var imageDao = new ImageDAO(testImg, name, creationTime);

            foreach (var ob in observations)
            {
                imageDao.Add(new ObservationDAO(imageDao, ob));
            }

            return (imageDao, JSONService.ConvertToJson(imageDao));
        }

        [Test]
        public void TestJsonSerialization()
        {
            var (expectedObject, json) = GenerateJsonsExamples();
            var actualObject = JsonConvert.DeserializeObject<ImageDAO>(json);

            IsImageDaoEqual(expectedObject, actualObject);
        }

        private static void IsImageDaoEqual(ImageDAO objectA, ImageDAO objectB)
        {
            Assert.AreEqual(objectA.Id, objectB.Id);
            Assert.AreEqual(objectA.Base64, objectB.Base64);

            for (var i = 0; i < (objectA.Observations).Count; i++)
            {
                IsObservationDaoEqual(objectA.Observations[i], objectB.Observations[i]);
            }
        }

        private static void IsObservationDaoEqual(ObservationDAO objectA, ObservationDAO objectB)
        {
            Assert.AreEqual(objectA.Id, objectB.Id);
            Assert.AreEqual(objectA.Confidence, objectB.Confidence, Epislon);
            Assert.AreEqual(objectA.CategoryId, objectB.CategoryId);
            Assert.AreEqual(objectA.BoundBoxTop, objectB.BoundBoxTop, Epislon);
            Assert.AreEqual(objectA.BoundBoxLeft, objectB.BoundBoxLeft, Epislon);
            Assert.AreEqual(objectA.BoundBoxRight, objectB.BoundBoxRight, Epislon);
            Assert.AreEqual(objectA.BoundBoxBottom, objectB.BoundBoxBottom, Epislon);
        }
    }
}
