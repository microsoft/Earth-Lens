using System;
using EarthLens.Models;
using LiteDB;
using Newtonsoft.Json;

namespace EarthLens.ORM
{
    public class ObservationDAO
    {
        [JsonProperty(SharedConstants.JsonId)]
        public int Id { get; set; }
        
        [JsonProperty(SharedConstants.JsonConfidence)]
        public double Confidence { get; set; }
        
        [JsonProperty(SharedConstants.JsonCategoryId)]
        public int CategoryId { get; set; }
        
        [JsonProperty(SharedConstants.JsonCategoryLabel)]
        public string CategoryLabel { get; set; }
        
        [JsonProperty(SharedConstants.JsonBoundBoxLeft)]
        public double BoundBoxLeft { get; set; }
        
        [JsonProperty(SharedConstants.JsonBoundBoxRight)]
        public double BoundBoxRight { get; set; }
        
        [JsonProperty(SharedConstants.JsonBoundBoxTop)]
        public double BoundBoxTop { get; set; }
        
        [JsonProperty(SharedConstants.JsonBoundBoxBottom)]
        public double BoundBoxBottom { get; set; }
    
        [JsonProperty(SharedConstants.JsonUploadDate)]
        public DateTime UploadDateTime { get; set; }

        [JsonIgnore]
        [BsonRef(SharedConstants.ImageCollectionName)]
        public ImageDAO Image { get; set; }

        public ObservationDAO() {}

        public ObservationDAO(ImageDAO image, Observation observation)
        {
            if (observation == null)
            {
                throw new ArgumentNullException();
            }

            Image = image;
            Confidence = observation.Confidence;
            CategoryId = observation.Category.Id;
            CategoryLabel = observation.Category.Label;
            BoundBoxLeft = observation.BoundingBox.Left;
            BoundBoxRight = observation.BoundingBox.Right;
            BoundBoxTop = observation.BoundingBox.Top;
            BoundBoxBottom = observation.BoundingBox.Bottom;
            UploadDateTime = DateTime.UtcNow;
        }

        public Observation ToObservation()
        {
            return new Observation (
                new Category(CategoryId, CategoryLabel),
                Confidence,
                new SkiaSharp.SKRect(
                    (float) BoundBoxLeft,
                    (float) BoundBoxTop,
                    (float) BoundBoxRight,
                    (float) BoundBoxBottom
                )
            );
        }
        
        protected bool Equals(ObservationDAO other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ObservationDAO) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }        
    }
}