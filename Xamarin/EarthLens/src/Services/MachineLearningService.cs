using System;
using System.Collections.Generic;
using System.Linq;

namespace EarthLens.Services
{
    public static class MachineLearningService
    {
        /// <summary>
        /// Convert a list of logit into confidence score ranging 0 to 1 through softmax conversion
        /// </summary>
        /// <returns>A list of double representing the confidence </returns>
        /// <param name="input"> A list of doubles representing logit output from the model </param>
        /// Side effect: the original logit input array is altered to be the confidence percentage
        public static IEnumerable<double> Softmax(IEnumerable<double> input)
        {
            var inputList = input.ToList();
            if (!inputList.Any())
            {
                return new List<double>();
            }
            var result = inputList.Select(item => Math.Exp(item)).ToList();

            var sum = result.Sum();
            return result.Select(item => (item / sum));
        }
    }
}
