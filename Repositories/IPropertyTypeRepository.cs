using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public interface IPropertyTypeRepository
    {
        List<PropertyType> GetAll();
        PropertyType GetById(int id);
        void Add(PropertyType propertyType);
        void Update(PropertyType propertyType);
        void Delete(int id);
        void Save();
    }
}
