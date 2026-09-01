using RealEstateSystem.Data;
using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public class PropertyTypeRepository : IPropertyTypeRepository
    {
        private readonly ApplicationDbContext context;

        public PropertyTypeRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<PropertyType> GetAll()
        {
            return context.PropertyTypes.ToList();
        }

        public PropertyType GetById(int id)
        {
            return context.PropertyTypes.FirstOrDefault(t => t.Id == id);
        }

        public void Add(PropertyType propertyType)
        {
            context.PropertyTypes.Add(propertyType);
        }

        public void Update(PropertyType propertyType)
        {
            context.PropertyTypes.Update(propertyType);
        }

        public void Delete(int id)
        {
            PropertyType propertyType = context.PropertyTypes.FirstOrDefault(t => t.Id == id);
            if (propertyType != null)
                context.PropertyTypes.Remove(propertyType);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
