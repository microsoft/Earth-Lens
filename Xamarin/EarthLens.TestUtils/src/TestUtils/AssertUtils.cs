using System;
using System.Collections.Generic;
using System.Linq;
using EarthLens.Models;
using NUnit.Framework;
using SkiaSharp;

namespace EarthLens.Tests.TestUtils
{
    public static class AssertUtils
    {
        private const double Tolerance = 1e-6;

        /// <summary>
        /// Checks if the two specified <see cref="SKRect"/> are equal within the tolerance.
        /// </summary>
        /// <param name="expected">The expected <see cref="SKRect"/>.</param>
        /// <param name="actual">The actual <see cref="SKRect"/>.</param>
        public static void AreEqualSKRect(SKRect expected, SKRect actual)
        {
            Assert.AreEqual(expected.Left, actual.Left, Tolerance);
            Assert.AreEqual(expected.Top, actual.Top, Tolerance);
            Assert.AreEqual(expected.Right, actual.Right, Tolerance);
            Assert.AreEqual(expected.Bottom, actual.Bottom, Tolerance);
        }

        /// <summary>
        /// Checks if the two specified <see cref="Observation"/> are equal.
        /// </summary>
        /// <param name="expected">The expected <see cref="Observation"/>.</param>
        /// <param name="actual">The actual <see cref="Observation"/>.</param>
        public static void AreEqualObservation(Observation expected, Observation actual)
        {
            Assert.AreEqual(expected.Category, actual.Category);
            Assert.AreEqual(expected.Confidence, actual.Confidence, Tolerance);
            AreEqualSKRect(expected.BoundingBox, actual.BoundingBox);
        }

        /// <summary>
        /// Checks if the two specified <see cref="IEnumerable{T}"/>s are equal.
        /// </summary>
        /// <param name="expected">The expected <see cref="IEnumerable{T}"/>.</param>
        /// <param name="actual">The actual <see cref="IEnumerable{T}"/>.</param>
        /// <param name="asserter">The optional method for assertion of two elements of type <see cref="T"/>.</param>
        /// <typeparam name="T">The type of elements of the two specified <see cref="IEnumerable{T}"/>s.</typeparam>
        public static void AreEqualIEnumerable<T>(IEnumerable<T> expected, IEnumerable<T> actual, Action<T, T> asserter
            = null)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            Assert.AreEqual(expectedList.Count, actualList.Count);

            for (var i = 0; i < expectedList.Count; i++)
            {
                if (asserter == null)
                {
                    Assert.AreEqual(expectedList[i], actualList[i]);
                }
                else
                {
                    asserter(expectedList[i], actualList[i]);
                }
            }
        }
    }
}
