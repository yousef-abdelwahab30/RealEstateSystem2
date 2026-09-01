using RealEstateSystem.Data;
using RealEstateSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstateSystem.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly ApplicationDbContext context;

        public PropertyImageRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<PropertyImage> GetAll()
        {
            return context.PropertyImages
                .Include(i => i.Property)
                .ToList();
        }

        public PropertyImage GetById(int id)
        {
            return context.PropertyImages
                .Include(i => i.Property)
                .FirstOrDefault(i => i.Id == id);
        }

        public List<PropertyImage> GetByProperty(int propertyId)
        {
            return context.PropertyImages
                .Where(i => i.PropertyId == propertyId)
                .OrderBy(i => i.DisplayOrder)
                .ToList();
        }

        public void Add(PropertyImage propertyImage)
        {
            context.PropertyImages.Add(propertyImage);
        }

        public void Update(PropertyImage propertyImage)
        {
            context.PropertyImages.Update(propertyImage);
        }

        public void Delete(int id)
        {
            PropertyImage propertyImage = context.PropertyImages.FirstOrDefault(i => i.Id == id);
            if (propertyImage != null)
                context.PropertyImages.Remove(propertyImage);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
