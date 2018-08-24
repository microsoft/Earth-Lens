using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace EarthLens
{
    public static class EnvironmentService
    {
        /// <summary>
        /// Sets environment variables from the specified resource in <see cref="SharedConstants"/>.
        /// </summary>
        public static void SetEnvironmentVariables()
        {
            var assembly = typeof(EnvironmentService).GetTypeInfo().Assembly;

            var prefix = Assembly.GetEntryAssembly().GetName().Name;
            var resourceName = string.Format(CultureInfo.InvariantCulture, SharedConstants.ResourceStringFormat, prefix,
                SharedConstants.EnvironmentResouceName);
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            var keyValueStrings = new List<string>();
            using (var reader = new StringReader(text))
            {
                while (true)
                {
                    var keyValueString = reader.ReadLine();
                    if (keyValueString != null)
                    {
                        keyValueStrings.Add(keyValueString.Trim());
                    }
                    else
                    {
                        break;
                    }
                }
            }

            SetEnvironmentVariables(keyValueStrings.ToArray());
        }

        /// <summary>
        /// Sets environment variables given by the spcified <see cref="string"/>s.
        /// Each string should be in the format "key=value".
        /// </summary>
        /// <param name="keyValueStrings">An array of key-value pairs seperated by '='.</param>
        private static void SetEnvironmentVariables(IEnumerable<string> keyValueStrings)
        {
            foreach (var keyValueString in keyValueStrings)
            {
                var keyValue = keyValueString.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (keyValue.Length != 2)
                {
                    continue;
                }

                var key = keyValue[0];
                var value = keyValue[1];

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
