using Microsoft.EntityFrameworkCore;
using CoffeeBeanApi.Models;

namespace CoffeeBeanApi.Data
{
    public class CoffeeBeanContext : DbContext
    {
        public CoffeeBeanContext(DbContextOptions<CoffeeBeanContext> options) : base(options)
        {
        }

        public DbSet<CoffeeBean> CoffeeBeans { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Colour> Colours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoffeeBean>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalId).HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

                entity.HasOne(e => e.Country)
                      .WithMany(c => c.CoffeeBeans)
                      .HasForeignKey(e => e.CountryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Colour)
                      .WithMany(c => c.CoffeeBeans)
                      .HasForeignKey(e => e.ColourId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.CountryId);
                entity.HasIndex(e => e.ColourId);
                entity.HasIndex(e => e.IsBeanOfTheDay);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Colour>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });
        }
    }
}