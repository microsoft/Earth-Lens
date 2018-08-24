using EarthLens.Models;
using NUnit.Framework;
           
namespace EarthLens.Tests.Models
{
    [TestFixture]
    public class MegaCategoryTests
    {
        private CategoryManager _categoryManager;
        private Category _category1;
        private string _category1MegaClassLabel;
        private Category _category2;
        private string _category2MegaClassLabel;
        private Category _category3;
        private string _category3MegaClassLabel;
        private Category _category4;

        [SetUp]
        public void SetUp()
        {
            _categoryManager = new CategoryManager();
            _category1 = _categoryManager.GetOrCreate(18, "Small Car");
            _category1MegaClassLabel = "Civilian Vehicles";
            _category2 = _categoryManager.GetOrCreate(70, "Building");
            _category2MegaClassLabel = "Structures & Sites";
            _category3 = new Category(70, "Apple");
            _category3MegaClassLabel = "Custom";
            _category4 = _categoryManager.GetOrCreate(18, "Bus");
        }

        [Test]
        public void TestGetMetaCategories()
        {
            Assert.AreEqual(_category1.MegaCategory.Label, _category1MegaClassLabel);
            Assert.AreEqual(_category2.MegaCategory.Label, _category2MegaClassLabel);
            Assert.AreEqual(_category3.MegaCategory.Label, _category3MegaClassLabel);
        }

        [Test]
        public void TestMetaCategoryEqual()
        {
            Assert.AreEqual(_category1.MegaCategory, _category4.MegaCategory);
        }
    }
}