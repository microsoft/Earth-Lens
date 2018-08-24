using System.Linq;
using EarthLens.Models;
using EarthLens.Services;
using EarthLens.Tests.TestUtils;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.Services
{
    [TestFixture]
    public class PostProcessingServiceTests
    {
        private const double IOUThreshold = 0.5;
        private const double ConfidenceThreshold = .35;

        private CategoryManager _categoryManager;
        private Category _category;

        [SetUp]
        public void SetUp()
        {
            _categoryManager = new CategoryManager();
            _category = _categoryManager.GetOrCreate(1, "Car");
        }

        [Test]
        public void TestProcessChipResults()
        {
            var offset = new SKPointI(300, 600);
            var observations = new[]
            {
                new Observation(_categoryManager.GetOrCreate(1, "Car"), 0.9, new SKRect(0.1f, 0.2f, 0.3f, 0.4f)),
                new Observation(_categoryManager.GetOrCreate(2, "Building"), 0.78, new SKRect(.2f, 0.3f, 0.5f, 0.8f))
            };

            PostProcessingService.ProcessChipResults(offset, observations);

            AssertUtils.AreEqualIEnumerable(new[]
            {
                new Observation(
                    _categoryManager.GetOrCreate(1, "Car"),
                    0.9,
                    new SKRect(offset.X + 0.1f * SharedConstants.DefaultChipWidth,
                        offset.Y + 0.2f * SharedConstants.DefaultChipHeight,
                        offset.X + 0.3f * SharedConstants.DefaultChipWidth,
                        offset.Y + 0.4f * SharedConstants.DefaultChipHeight)),
                new Observation(
                    _categoryManager.GetOrCreate(2, "Building"),
                    0.78,
                    new SKRect(offset.X + 0.2f * SharedConstants.DefaultChipWidth,
                        offset.Y + 0.3f * SharedConstants.DefaultChipHeight,
                        offset.X + 0.5f * SharedConstants.DefaultChipWidth,
                        offset.Y + 0.8f * SharedConstants.DefaultChipHeight)),
            },
                observations,
                AssertUtils.AreEqualObservation);
        }

        [Test]
        public void TestNonMaximumSuppression_NonOverlapping()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(0.1f, 0.2f, 0.3f, 0.4f));
            var observation2 = new Observation(_category, 0.78, new SKRect(0.4f, 0.5f, 0.5f, 0.6f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(observations, result);
        }

        [Test]
        public void TestNonMaximumSuppression_IOUSmallerThanThreshold()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(0.1f, 0.2f, 0.3f, 0.4f));
            var observation2 = new Observation(_category, 0.78, new SKRect(0.2f, 0.3f, 0.5f, 0.6f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(observations, result);
        }

        [Test]
        public void TestNonMaximumSuppression_IOUEqualToThreshold()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(0f, 0f, 6f, 6f));
            var observation2 = new Observation(_category, 0.78, new SKRect(2f, 0f, 8f, 6f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(observations, result);
        }

        [Test]
        public void TestNonMaximumSuppression_IOUGreaterThanThreshold()
        {
            var observation1 = new Observation(_category, 0.78, new SKRect(1f, 1f, 7f, 7f));
            var observation2 = new Observation(_category, 0.9, new SKRect(0f, 0f, 6f, 6f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(new[] {observation2}, result);
        }

        [Test]
        public void TestNonMaximumSuppression_CompletelyOverlappingIOULessThanThreshold()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(1f, 2f, 3f, 4f));
            var observation2 = new Observation(_category, 0.78, new SKRect(1f, 2f, 10f, 20f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(observations, result);
        }

        [Test]
        public void TestNonMaximumSuppression_CompletelyOverlappingIOUEqualToThreshold()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(1f, 2f, 5f, 8f));
            var observation2 = new Observation(_category, 0.78, new SKRect(2f, 3f, 5f, 7f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(observations, result);
        }

        [Test]
        public void TestNonMaximumSuppression_CompletelyOverlappingIOUGreaterThanThreshold()
        {
            var observation1 = new Observation(_category, 0.78, new SKRect(1f, 2f, 5f, 8f));
            var observation2 = new Observation(_category, 0.9, new SKRect(1.5f, 2.5f, 5f, 7.5f));
            var observations = new[] {observation1, observation2};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(new[] {observation2}, result);
        }

        [Test]
        public void TestNonMaximumSuppression_ThreeObservations1()
        {
            var observation1 = new Observation(_category, 0.78, new SKRect(1f, 1f, 7f, 7f));
            var observation2 = new Observation(_category, 0.9, new SKRect(0f, 0f, 6f, 6f));
            var observation3 = new Observation(_category, 0.8, new SKRect(2f, 2f, 8f, 8f));
            var observations = new[] {observation1, observation2, observation3};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(new[] {observation2, observation3}, result);
        }

        [Test]
        public void TestNonMaximumSuppression_ThreeObservations2()
        {
            var observation1 = new Observation(_category, 0.9, new SKRect(1f, 1f, 7f, 7f));
            var observation2 = new Observation(_category, 0.78, new SKRect(0f, 0f, 6f, 6f));
            var observation3 = new Observation(_category, 0.8, new SKRect(2f, 2f, 8f, 8f));
            var observations = new[] {observation1, observation2, observation3};

            var result = PostProcessingService.NonMaximumSuppression(observations, IOUThreshold, ConfidenceThreshold)
                .ToList();

            AssertUtils.AreEqualIEnumerable(new[] {observation1}, result);
        }
    }
}
