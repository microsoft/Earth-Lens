using System;
using System.Collections.Generic;
using System.IO;
using EarthLens.Models;
using EarthLens.ORM;
using LiteDB;

namespace EarthLens.Services
{
    public static class DatabaseService
    {
        /// <summary>
        /// Retrieves all images from the database.
        /// </summary>
        /// <returns>All Images in the format of <see cref="ImageDAO"/>.</returns>
        public static IEnumerable<ImageDAO> GetAllImages()
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                return db.GetCollection<ImageDAO>(SharedConstants.ImageCollectionName).FindAll();
            }
        }

        /// <summary>
        /// Creates table instances of the <see cref="ObservationDAO"/> and <see cref="ImageDAO"/>
        /// </summary>
        public static void CreateTables()
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                db.GetCollection<ImageDAO>(SharedConstants.ImageCollectionName);
                db.GetCollection<ObservationDAO>(SharedConstants.ObservationCollectionName);
            }
        }

        /// <summary>
        /// Drops table instances of the <see cref="ObservationDAO"/> and <see cref="ImageDAO"/>
        /// </summary>
        public static void DropTables()
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                db.DropCollection(SharedConstants.ImageCollectionName);
                db.DropCollection(SharedConstants.ObservationCollectionName);
            }
        }

        /// <summary>
        /// Deletes an ImageDAO, if it exists, and all of its associated Observations
        /// </summary>
        public static void DeleteImage(int id)
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                var imageCollection = db.GetCollection<ImageDAO>(SharedConstants.ImageCollectionName);
                imageCollection.Delete(id);

                var observationCollection = db.GetCollection<ObservationDAO>(SharedConstants.ObservationCollectionName);
                observationCollection.Delete(x => x.Image.Id == id);
            }
        }

        /// <summary>
        /// Returns a specfied ImageDAO, if it exists
        /// and returns null if the ImageDAO is not found
        /// </summary>
        public static ImageDAO GetImage(int id)
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                var imageCollection = db.GetCollection<ImageDAO>(SharedConstants.ImageCollectionName);              
                return imageCollection.FindById(id);
            }
        }

        /// <summary>
        /// Adds base64 representation of SKImage and Observations to DB,
        /// or updates an already existing image within the DB
        /// </summary>
        /// <see cref="ImageDAO"/> and <see cref="ObservationDAO"/>
        public static ImageDAO InsertOrUpdate(ImageEntry imageEntry)
        {
            if (imageEntry == null)
            {
                throw new ArgumentNullException();
            }

            using (var db = CreateOrGetLiteDBConnection())
            {
                var img = imageEntry.Image;
                var name = imageEntry.Name;
                var creationTime = imageEntry.CreationTime ?? DateTime.UtcNow;
                var observations = imageEntry.Observations;
                
                var imageCollection = db.GetCollection<ImageDAO>(SharedConstants.ImageCollectionName);
                var observationCollection = db.GetCollection<ObservationDAO>(SharedConstants.ObservationCollectionName);
                
                var imageDao = new ImageDAO(img, name, creationTime);
                var observationDaos = new List<ObservationDAO>();
                
                if (imageEntry.Id == null)
                {
                    AddObservationsToImgDao(observations, imageDao, observationDaos);
                    
                    imageCollection.Insert(imageDao);
                    observationCollection.Insert(observationDaos);
                }
                else
                {
                    imageDao.Id = (int) imageEntry.Id;
                    
                    AddObservationsToImgDao(observations, imageDao, observationDaos);
                    
                    imageCollection.Update(imageDao);
                    observationCollection.Delete(x => x.Image.Id == imageEntry.Id);
                    observationCollection.Insert(observationDaos);
                }
                
                return imageDao;
            }
        }

        private static void AddObservationsToImgDao(IEnumerable<Observation> observations, ImageDAO imageDao, ICollection<ObservationDAO> observationDaos)
        {
            foreach (var obs in observations)
            {
                var observationDao = new ObservationDAO(imageDao, obs);
                imageDao.Add(observationDao);
                observationDaos.Add(observationDao);
            }
        }

        /// <summary>
        /// Updates Database schema according to specified changes, and updates the user version
        /// </summary>
        public static void MigrateDBColumns()
        {
            using (var db = CreateOrGetLiteDBConnection())
            {
                if(db.Engine.UserVersion == 0)
                {
                    //placeholder for schema changes, update UserVersion aftewards
                }
            }    
        }
        
        /// <summary>
        /// Creates a new instance of a LiteDB connection, or retrieves an existing connection
        /// </summary>
        public static LiteDatabase CreateOrGetLiteDBConnection()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                SharedConstants.DBName);

            var db = new LiteDatabase(dbPath);
            return db;
        }
    }
}     