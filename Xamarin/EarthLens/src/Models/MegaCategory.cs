using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace EarthLens.Models
{
    public class MegaCategory
    {
        public string Label { get; }

        private static Dictionary<string, MegaCategory> _mapping;

        private MegaCategory(string label)
        {
            Label = label;
        }

        /// <summary>
        /// Gets the meta category mapping.
        /// Should only be used by the <see cref="Category"> class.
        /// </summary>
        /// <returns>The meta category mapping.</returns>
        /// <param name="category">The Category for which the <see cref="MegaCategory"/> is queried.</param>
        public static MegaCategory GetMegaCategoryMapping(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException();
            }

            if (_mapping == null)
            {
                _mapping = Initialize();
            }

            return _mapping.TryGetValue(category.Label, out var megaClass) ?
                           megaClass : _mapping[SharedConstants.CustomMegaCategoryLabel];
        }

        /// <summary>
        /// Parsing from the resource Json file to obtain the mapping
        /// </summary>
        private static Dictionary<string, MegaCategory> Initialize()
        {
            var mapping = new Dictionary<string, MegaCategory>();

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MegaCategory)).Assembly;
            var stream = assembly.GetManifestResourceStream(SharedConstants.MegaCategoryMappingJsonFilePath);
            if (stream == null)
            {
                throw new ArgumentNullException();
            }
            var r = new StreamReader(stream);

            var json = r.ReadToEnd();
            var mappingObjectItems = JsonConvert.DeserializeObject<List<DeserializationMapping>>(json);

            foreach(var item in mappingObjectItems)
            {
                var megaCategory = new MegaCategory(item.Name);
                foreach( var subCat in item.SubCategories)
                {
                    mapping.Add(subCat, megaCategory);
                }
            }
       
            mapping.Add(SharedConstants.CustomMegaCategoryLabel, new MegaCategory(SharedConstants.CustomMegaCategoryLabel));
            return mapping;
        }

        protected bool Equals(MegaCategory other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            return string.Equals(Label, other.Label, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MegaCategory) obj);
        }

        public override int GetHashCode()
        {
            return (Label != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Label) : 0);
        }

        private class DeserializationMapping
        {
            public string Name { get; set; }
            public List<string> SubCategories { get; set; }
        }
    }
}