using Microsoft.EntityFrameworkCore;

namespace Cleanup.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public virtual DbSet<FileStorage> FileStorage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<FileStorage>(entity =>
            {
                entity.HasKey(e => e.FileStorageId);

                entity.ToTable("filestorage");

                entity.Property(e => e.FileStorageId).HasColumnName("filestorage_id");

                entity.Property(e => e.FileStorageGuid).HasColumnName("filestorage_guid");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasColumnName("filename")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });
        }
    }
}
