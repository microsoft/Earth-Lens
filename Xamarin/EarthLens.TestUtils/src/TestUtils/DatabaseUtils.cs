using System.Collections.Generic;
using EarthLens.ORM;
using EarthLens.Services;

namespace EarthLens.Tests.TestUtils
{
    public static class DatabaseUtils
    {
        public static IEnumerable<ObservationDAO> GetAllObservations()
        {
            using (var db = DatabaseService.CreateOrGetLiteDBConnection())
            {
                return db.GetCollection<ObservationDAO>(SharedConstants.ObservationCollectionName).FindAll();
            }
        }
    }
}