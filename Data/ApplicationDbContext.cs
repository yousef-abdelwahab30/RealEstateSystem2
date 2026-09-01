using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RealEstateSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(t => t.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.City)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Agent)
                .WithMany(a => a.Properties)
                .HasForeignKey(p => p.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyImage>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Property)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "role-agent", Name = "Agent", NormalizedName = "AGENT" },
                new IdentityRole { Id = "role-customer", Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            modelBuilder.Entity<PropertyType>().HasData(
                new PropertyType { Id = 1, Name = "Apartment", IsActive = true },
                new PropertyType { Id = 2, Name = "Villa", IsActive = true },
                new PropertyType { Id = 3, Name = "Duplex", IsActive = true },
                new PropertyType { Id = 4, Name = "Studio", IsActive = true },
                new PropertyType { Id = 5, Name = "Office", IsActive = true },
                new PropertyType { Id = 6, Name = "Shop", IsActive = true },
                new PropertyType { Id = 7, Name = "Land", IsActive = true }
            );

            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Cairo", Governorate = "Cairo" },
                new City { Id = 2, Name = "Giza", Governorate = "Giza" },
                new City { Id = 3, Name = "Alexandria", Governorate = "Alexandria" },
                new City { Id = 4, Name = "Mansoura", Governorate = "Dakahlia" },
                new City { Id = 5, Name = "New Cairo", Governorate = "Cairo" },
                new City { Id = 6, Name = "6th of October", Governorate = "Giza" },
                new City { Id = 7, Name = "Sheikh Zayed", Governorate = "Giza" },
                new City { Id = 8, Name = "Hurghada", Governorate = "Red Sea" }
            );

            modelBuilder.Entity<Agent>().HasData(
                new Agent { Id = 1, FullName = "Ahmed Hassan", Phone = "01001234567", Email = "ahmed@realestate.com", AgencyName = "Hassan Realty" },
                new Agent { Id = 2, FullName = "Sara Mahmoud", Phone = "01112345678", Email = "sara@realestate.com", AgencyName = "Prime Homes" },
                new Agent { Id = 3, FullName = "Omar Adel", Phone = "01223456789", Email = "omar@realestate.com", AgencyName = "City Estates" }
            );

            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Title = "Sea view apartment in Alexandria",
                    Description = "Bright 3-bedroom apartment with a large balcony overlooking the sea.",
                    PropertyTypeId = 1, CityId = 3, AgentId = 1,
                    Address = "Corniche Road, Sidi Gaber",
                    Price = 3500000, Area = 165, Bedrooms = 3, Bathrooms = 2,
                    ListingType = ListingType.Sale, Status = PropertyStatus.Approved,
                    IsFurnished = true, CreatedAt = new DateTime(2026, 1, 15)
                },
                new Property
                {
                    Id = 2,
                    Title = "Modern villa in Sheikh Zayed",
                    Description = "Spacious villa with private garden and swimming pool.",
                    PropertyTypeId = 2, CityId = 7, AgentId = 2,
                    Address = "Beverly Hills Compound",
                    Price = 12000000, Area = 420, Bedrooms = 5, Bathrooms = 4,
                    ListingType = ListingType.Sale, Status = PropertyStatus.Approved,
                    IsFurnished = false, CreatedAt = new DateTime(2026, 2, 3)
                },
                new Property
                {
                    Id = 3,
                    Title = "Studio for rent in New Cairo",
                    Description = "Fully furnished studio, ready to move in.",
                    PropertyTypeId = 4, CityId = 5, AgentId = 3,
                    Address = "90th Street, Fifth Settlement",
                    Price = 9000, Area = 55, Bedrooms = 1, Bathrooms = 1,
                    ListingType = ListingType.Rent, Status = PropertyStatus.Approved,
                    IsFurnished = true, CreatedAt = new DateTime(2026, 3, 10)
                },
                new Property
                {
                    Id = 4,
                    Title = "Office space in Downtown Cairo",
                    Description = "Open plan office on the fourth floor, close to the metro.",
                    PropertyTypeId = 5, CityId = 1, AgentId = 1,
                    Address = "Talaat Harb Square",
                    Price = 25000, Area = 200, Bedrooms = 0, Bathrooms = 2,
                    ListingType = ListingType.Rent, Status = PropertyStatus.Pending,
                    IsFurnished = false, CreatedAt = new DateTime(2026, 4, 1)
                }
            );
        }
    }
}
