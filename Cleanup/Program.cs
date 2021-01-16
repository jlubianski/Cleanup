using Cleanup.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SST.ORBB.Cleanup.CDN
{
    class Program
    {
        private static readonly Logger logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
        private static string PATH;
        private static string DATABASE;
        private static int EXPIRY_DAYS;

        public static IConfiguration Configuration { get; set; }

        static void Main()
        {
            try
            {
                //read app settings
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                Configuration = builder.Build();

                PATH = Configuration.GetSection("Path").Value;
                DATABASE = $"{Configuration["ConnectionStrings:Database"]}";
                EXPIRY_DAYS = int.Parse(Configuration.GetSection("ExpiryDays").Value);

                CleanupFiles();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
        }

        private static void CleanupFiles()
        {
            DbContextOptions _dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(DATABASE).Options;
            DateTime expiryDate = DateTime.Now.AddDays(-EXPIRY_DAYS);

            //get all the expired files from db
            List<FileStorage> fileStorages = FileStorage.GetAll(expiryDate, _dbContextOptions);
            List<string> dirPaths = new List<string>();  //for removal

            foreach (FileStorage fileStorage in fileStorages)
            {
                logger.Info("Processing... " + fileStorage.Filename);

                //delete all files
                try
                {
                    string filePath = PATH + fileStorage.Path;
                    if (Directory.Exists(filePath))
                    {
                        string fileToRemove = Directory.EnumerateFiles(filePath, fileStorage.Filename).SingleOrDefault();
                        
                        if (File.Exists(fileToRemove))
                        {
                            File.Delete(fileToRemove);
                            logger.Info("Removed... " + fileStorage);
                        }
                    }

                    //compile list of directories to remove
                    dirPaths.Add(filePath);

                    fileStorage.Delete(fileStorage.FileStorageGuid, _dbContextOptions);
                    logger.Info("Removal complete for" + fileStorage.Filename);
                }
                catch (Exception ex)
                {
                    logger.Error("Error while removing file... " + fileStorage.Filename);
                    logger.Error(ex.Message);
                    logger.Error(ex.StackTrace);
                }
            }

            //remove directories that are empty
            foreach (string dir in dirPaths)
            {
                if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    Directory.Delete(dir);
                }
            }
        }
    }
}
