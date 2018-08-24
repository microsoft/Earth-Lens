using System.Collections.Generic;
using System.Linq;
using EarthLens.Services;
using NUnit.Framework;

namespace EarthLens.Tests
{
    [TestFixture]
    public class MachingLearningServiceTests
    {
        private static double Epsilon = 1e-7;

        private static object[] TestCases =
        {
            new object[]{ new List<double>(){-9f, 8f, 0f, 3f, -1.5f, 2.45f }}, 
            new object[]{ new List<double>(){3f, 0f, 0.387f, 1f, 3.2f }},  
        };

        [TestCaseSource("TestCases")]  // passing through testCaseSource, as NUnit does not allow passing of Enumerables
        public void TestSoftmaxOutput(IEnumerable<double> input)
        {
            var actualOutput = MachineLearningService.Softmax(input);
            var softmaxSum = actualOutput.Sum();

            // check if the softmax output sum to 1
            Assert.AreEqual(softmaxSum, 1.0f, Epsilon);

            // check if the softmax results respect the original ordinal information 
            var originalSorted = input.Select((x, i) => new KeyValuePair<double, double>(x, i))
                                      .OrderBy(x => x.Key);

            var originalIdx = originalSorted.Select(x => x.Value).ToList(); // the index of items that ranked in increasing value 

            var answerSorted = actualOutput.Select((x, i) => new KeyValuePair<double, double>(x, i))
                                           .OrderBy(x => x.Key);

            var answerIdx = answerSorted.Select(x => x.Value).ToList();  // the index of items that ranked in increasing value
            for (int i = 0; i < originalIdx.Count; i++)
            {
                Assert.AreEqual(originalIdx[i], answerIdx[i]);
            }
        }
    }
}
