using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public interface IPropertyImageRepository
    {
        List<PropertyImage> GetAll();
        PropertyImage GetById(int id);
        List<PropertyImage> GetByProperty(int propertyId);
        void Add(PropertyImage propertyImage);
        void Update(PropertyImage propertyImage);
        void Delete(int id);
        void Save();
    }
}
