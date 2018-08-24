using EarthLens.Models;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.Models
{
    [TestFixture]
    public class CategoryTests
    {
        private Category _category;
        private SKRect _rect1;
        private SKRect _rect2;
        private SKRect _rect3;
        private Observation _observation1;
        private Observation _observation2;
        private Observation _observation3;

        [SetUp]
        public void SetUp()
        {
            _category = (new CategoryManager()).GetOrCreate(18, "Small Car");

            _rect1 = new SKRect(0.1f, 0.2f, 0.3f, 0.4f);
            _rect2 = new SKRect(0.1f, 2.3f, 4.5f, 6.7f);
            _rect3 = new SKRect(0.2f, 0.3f, 0.5f, 0.7f);
        }

        [Test, Order(1)]
        public void TestConstructor()
        {
            Assert.AreEqual(18, _category.Id);
            Assert.AreEqual("Small Car", _category.Label);
            Assert.AreEqual(0, _category.Observations.Count);
        }

        [Test, Order(2)]
        public void TestAdd()
        {
            _observation1 = new Observation(_category, 0.7, _rect1);
            Assert.AreEqual(1, _category.Observations.Count);

            _observation2 = new Observation(_category, 0.9, _rect2);
            Assert.AreEqual(2, _category.Observations.Count);

            _observation3 = new Observation(_category, 0.8, _rect3);
            Assert.AreEqual(3, _category.Observations.Count);
        }

        [Test]
        public void TestRemove()
        {
            _observation1 = new Observation(_category, 0.7, _rect1);
            _category.Remove(_observation1);
            Assert.AreEqual(0, _category.Observations.Count);
        }

        [Test]
        public void TestRemove_DifferentReferenceSameContents()
        {
            _observation1 = new Observation(_category, 0.7, _rect1);
            var observation1Copy = new Observation(_category, 0.7, _rect1);
            _category.Remove(observation1Copy);
            Assert.AreEqual(0, _category.Observations.Count);
        }

        [Test]
        public void TestEquals_SameReference()
        {
            var categoryManager = new CategoryManager();
            var category1 = categoryManager.GetOrCreate(1, "Small Car");
            var category2 = categoryManager.GetOrCreate(1, "Small Car");

            Assert.AreEqual(category1, category2);
        }

        [Test]
        public void TestEquals_DifferentReferenceSameContents()
        {
            var category1 = (new CategoryManager()).GetOrCreate(1, "Small Car");
            var category2 = (new CategoryManager()).GetOrCreate(1, "Small Car");

            Assert.AreEqual(category1, category2);
        }

        [Test]
        public void TestEquals_DifferentIds()
        {
            var category1 = (new CategoryManager()).GetOrCreate(1, "Small Car");
            var category2 = (new CategoryManager()).GetOrCreate(2, "Small Car");

            Assert.AreNotEqual(category1, category2);
        }

        [Test]
        public void TestEquals_DifferentLabels()
        {
            var category1 = (new CategoryManager()).GetOrCreate(1, "Small Car");
            var category2 = (new CategoryManager()).GetOrCreate(1, "Building");

            Assert.AreNotEqual(category1, category2);
        }

        [Test]
        public void TestColoursBetweenCategoires()
        {
            var category1 = (new CategoryManager()).GetOrCreate(1, "Small Car");
            var category2 = (new CategoryManager()).GetOrCreate(2, "Building");
            var category3 = (new CategoryManager()).GetOrCreate(23, "Randomthings");

            Assert.AreNotEqual(category1.Color, category2.Color);
            Assert.AreEqual(category3.Color, SharedConstants.DefaultBoundingBoxColor);
        }
    }
}
