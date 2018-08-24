using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace EarthLens.iOS.Services
{
    /// <summary>
    /// A class that parses the static id and class labels
    /// </summary>
    public static class ModelClassLabelEncoding
    {
        private static readonly IList<Tuple<int, string>> MappingList = Initialize();

        private static List<Tuple<int, string>> Initialize()
        {
            var mapping = new List<Tuple<int, string>>();

            var idRegex = new Regex (Constants.IDRegexString) ;
            var labelRegex = new Regex(Constants.LabelRegexString);

            var assembly = typeof(ModelClassLabelEncoding).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(Constants.ClassLabelMapResourceName);
  
            string text;
            using (var reader = new StreamReader(stream ?? throw new ArgumentNullException()))
            {
                text = reader.ReadToEnd();
            }
            var lines = text.Split('\n');

            var ids = new List<int>();
            var labels = new List<string>();

            foreach (var line in lines)
            {
                var idMatch = idRegex.Match(line);
                var labelMatch = labelRegex.Match(line);

                if (idMatch.Success) // check for id matching results
                {
                    var temp = idMatch.Groups[0].Value;
                    if (int.TryParse(temp, out var parsedInt))
                    {
                        ids.Add(parsedInt);
                    }
                }

                if (!labelMatch.Success) continue;
                {
                    var temp = labelMatch.Groups[0].Value;
                    labels.Add(temp);
                }
            }

            if (ids.Count != labels.Count)
            {
                return null;
            }

            // formulize to key-value pair
            mapping.Add(Tuple.Create(0, "background")); // add one extra label for background
            mapping.AddRange(labels.Select((t, i) => Tuple.Create(ids[i], t)));
            return mapping;
        }

        public static string LookUpLabel(int id)
        {
            return MappingList[id].Item2;
        }
    }
}
