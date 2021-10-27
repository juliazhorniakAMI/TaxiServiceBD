using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TaxiServiceBD.Models
{
    public partial class TaxiServiceContext : DbContext
    {
        public TaxiServiceContext()
        {
        }

        public TaxiServiceContext(DbContextOptions<TaxiServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoriesClassDetail> CategoriesClassDetails { get; set; }
      
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
  
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<CategoriesTaxiClass> CategoriesTaxiClasses { get; set; }
        public virtual DbSet<TaxiClass> TaxiClasses { get; set; }
        public virtual DbSet<User> Users { get; set; }

      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CategoriesClassDetail>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.TaxiClassId).HasColumnName("TaxiClassID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoriesClassDetails)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoriesClassDetails_Categories");

                entity.HasOne(d => d.TaxiClass)
                    .WithMany(p => p.CategoriesClassDetails)
                    .HasForeignKey(d => d.TaxiClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoriesClassDetails_TaxiClass");
            });

            modelBuilder.Entity<CategoriesTaxiClass>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CategoriesTaxiClass");

                entity.Property(e => e.Category)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Taxi)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasIndex(e => e.PhoneNumber, "IX_DriversPhone")
                    .IsUnique();

                entity.HasIndex(e => e.FullName, "IX_Drivers_Name")
                    .IsUnique();

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

         

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CategoryClassId).HasColumnName("CategoryClassID");

                entity.Property(e => e.DateOfCreation).HasColumnType("date");

                entity.HasOne(d => d.CategoryClass)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CategoryClassId)
                    .HasConstraintName("FK_Orders_CategoriesClassDetails");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DriverId)
                    .HasConstraintName("FK__Orders__DriverId__30F848ED");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__300424B4");
            });

        

            modelBuilder.Entity<TaxiClass>(entity =>
            {
                entity.ToTable("TaxiClass");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.FullName, "IX_UsersName")
                    .IsUnique();

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
