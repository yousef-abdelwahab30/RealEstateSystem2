using RealEstateSystem.Data;
using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace RealEstateSystem.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext context;

        public PropertyRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<Property> GetAll()
        {
            return context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.Agent)
                .ToList();
        }

        public Property GetById(int id)
        {
            return context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.Agent)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);
        }

        public List<Property> Search(int? cityId, int? propertyTypeId, ListingType? listingType,
            decimal? minPrice, decimal? maxPrice, int? bedrooms)
        {
            var query = context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Where(p => p.Status == PropertyStatus.Approved);

            if (cityId.HasValue)
                query = query.Where(p => p.CityId == cityId);

            if (propertyTypeId.HasValue)
                query = query.Where(p => p.PropertyTypeId == propertyTypeId);

            if (listingType.HasValue)
                query = query.Where(p => p.ListingType == listingType);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (bedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= bedrooms);

            return query.ToList();
        }

        public void Add(Property property)
        {
            context.Properties.Add(property);
        }

        public void Update(Property property)
        {
            context.Properties.Update(property);
        }

        public void Delete(int id)
        {
            Property property = context.Properties.FirstOrDefault(p => p.Id == id);
            if (property != null)
                context.Properties.Remove(property);
        }

        public void Approve(int id)
        {
            Property property = context.Properties.FirstOrDefault(p => p.Id == id);
            if (property != null)
                property.Status = PropertyStatus.Approved;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
