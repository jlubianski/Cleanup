using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cleanup.Model
{
    public class FileStorage
    {
        public int FileStorageId { get; set; }
        public Guid FileStorageGuid { get; set; }
        public string Path { get; set; }
        public string Filename { get; set; }
        public DateTime CreateDate { get; set; }

        public static List<FileStorage> GetAll(DateTime expiryDate, DbContextOptions dbContextOptions)
        {
            using (var context = new DatabaseContext(dbContextOptions))
            {
                try
                {
                    return context.FileStorage.Where(a => a.CreateDate < expiryDate).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Delete(Guid assetGuid, DbContextOptions dbContextOptions)
        {
            using (var context = new DatabaseContext(dbContextOptions))
            {
                try
                {
                    FileStorage itemToDelete = context.FileStorage.Where(a => a.FileStorageGuid == assetGuid).SingleOrDefault();
                    context.FileStorage.Remove(itemToDelete);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
