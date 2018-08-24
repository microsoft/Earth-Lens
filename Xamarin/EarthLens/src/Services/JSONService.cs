using Newtonsoft.Json;
using EarthLens.ORM;

namespace EarthLens.Services
{
    public static class JSONService
    {
        /// <summary>
        /// Converts a ImageDAO object into a  json strings
        /// </summary>
        /// <returns>a list of json strings </returns>
        /// <param name="image"> <see cref="ImageDAO"/> objects</param>
        public static string ConvertToJson(ImageDAO image)
        {
            return JsonConvert.SerializeObject(image, Formatting.Indented);
        }
    }
}
