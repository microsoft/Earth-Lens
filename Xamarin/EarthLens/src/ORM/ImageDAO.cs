using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using EarthLens.Services;
using SkiaSharp;

namespace EarthLens.ORM
{
    public class ImageDAO
    {
        [JsonProperty(SharedConstants.JsonId)]
        public int Id { get; set; }
        
        [JsonProperty(SharedConstants.JsonBase64)]
        public string Base64 { get; set; }
        
        [JsonProperty(SharedConstants.JsonName)]
        public string Name { get; set; }
        
        [JsonProperty(SharedConstants.JsonCreationTime)]
        public DateTime CreationTime { get; set; }
        
        [JsonProperty(SharedConstants.JsonObservations)]
        public IList<ObservationDAO> Observations { get; set; }

        public ImageDAO() {}
        
        public ImageDAO(SKImage img, string name, DateTime creationTime)
        {
            Base64 = ImageEncodingService.SKImageToBase64(img);
            Name = name;
            CreationTime = creationTime;
            Observations = new List<ObservationDAO>();
        }

        public void Add(ObservationDAO observation)
        {
            Observations.Add(observation);
        }

        protected bool Equals(ImageDAO other)
        {
            if (other == null)
            {
                throw new ArgumentException();
            }
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ImageDAO) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}