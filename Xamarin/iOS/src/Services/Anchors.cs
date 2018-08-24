using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace EarthLens.iOS.Services
{
    /// <summary>
    /// A class that bundle all coordinates of an anchor together 
    /// </summary>
    public class Anchor
    {
        public double YMin { get; }
        public double XMin { get; }
        public double YMax { get; }
        public double XMax { get; }

        public Anchor(double ymin, double xmin, double ymax, double xmax)
        {
            YMin = ymin;
            XMin = xmin;
            YMax = ymax;
            XMax = xmax;
        }
    }

    public static class Anchors
    {
        private static readonly List<Anchor> AnchorList = ReadInAnchorsFromFile();

        public static Anchor GetBoxCoordinate(int index)
        {
            return AnchorList[index];
        }

        private static List<Anchor> ReadInAnchorsFromFile()
        {
            var assembly = typeof(ModelClassLabelEncoding).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(Constants.AnchorsResourceName);

            string text;
            using (var reader = new StreamReader(stream ?? throw new ArgumentNullException()))
            {
                text = reader.ReadToEnd();
            }
            var lines = text.Split('\n');

            return (from line in lines
                select line.Split(',')
                into entries
                select entries.Select(double.Parse).ToList()
                into boxDoubles
                select new Anchor(boxDoubles[0], boxDoubles[1], boxDoubles[2], boxDoubles[3])).ToList();
        }
    }
}
