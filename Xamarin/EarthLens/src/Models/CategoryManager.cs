using System.Collections.Generic;

namespace EarthLens.Models
{
    public class CategoryManager
    {
        private readonly Dictionary<int, Category> _dict;

        public IEnumerable<Category> Categories
        {
            get
            {
                var categories = new List<Category>();

                var categoryColl = _dict.Values;
                foreach (var category in categoryColl)
                {
                    categories.Add(category);
                }

                return categories;
            }
        }

        public IEnumerable<Observation> Observations
        {
            get
            {
                var observations = new List<Observation>();

                var categories = Categories;
                foreach (var category in categories)
                {
                    var categorbservations = category.Observations;
                    observations.AddRange(categorbservations);
                }

                return observations;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryManager"/> class.
        /// </summary>
        public CategoryManager()
        {
            _dict = new Dictionary<int, Category>();
        }

        /// <summary>
        /// If the specified ID has been bound to a <see cref="Category"/> instance, returns the <see cref="Category"/>.
        /// Otherwise, initializes a <see cref="Category"/> with the specified ID and label, and binds it with the ID.
        /// </summary>
        /// <returns>The <see cref="Category"/> with the specified ID and label.</returns>
        /// <param name="id">The ID of the <see cref="Category"/>.</param>
        /// <param name="label">The label of the <see cref="Category"/>.</param>
        public Category GetOrCreate(int id, string label)
        {
            if (_dict.TryGetValue(id, out var result))
            {
                return result;
            }

            result = new Category(id, label);
            _dict.Add(id, result);
            return result;
        }
    }
}
