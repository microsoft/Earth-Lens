using EarthLens.Models;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.Models
{
    [TestFixture]
    public class CategoryManagerTests
    {
        private CategoryManager _categoryManager;
        private Category _category1;
        private Category _category2;

        [SetUp]
        public void SetUp()
        {
            _categoryManager = new CategoryManager();
            _category1 = _categoryManager.GetOrCreate(18, "Small Car");
            _category2 = _categoryManager.GetOrCreate(70, "Building");
        }

        [Test]
        public void TestGetCategories()
        {
            Assert.AreEqual(new[] { _category1, _category2 }, _categoryManager.Categories);
        }

        [Test]
        public void TestGetObjects()
        {
            var observation1 = new Observation(_category1, 0.9, new SKRect(0.1f, 0.2f, 0.3f, 0.4f));
            var observation2 = new Observation(_category1, 0.78, new SKRect(0.5f, 0.6f, 0.7f, 0.8f));
            var observation3 = new Observation(_category2, 0.456, new SKRect(0.9f, 1.0f, 1.1f, 1.2f));

            Assert.AreEqual(new[] { observation1, observation2, observation3 },
                _categoryManager.Observations);
        }
    }
}
