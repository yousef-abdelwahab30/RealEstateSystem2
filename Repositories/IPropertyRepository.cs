using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;

namespace RealEstateSystem.Repositories
{
    public interface IPropertyRepository
    {
        List<Property> GetAll();
        Property GetById(int id);
        List<Property> Search(int? cityId, int? propertyTypeId, ListingType? listingType,
            decimal? minPrice, decimal? maxPrice, int? bedrooms);
        void Add(Property property);
        void Update(Property property);
        void Delete(int id);
        void Approve(int id);
        void Save();
    }
}
