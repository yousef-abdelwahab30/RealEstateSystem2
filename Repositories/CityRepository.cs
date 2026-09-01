using RealEstateSystem.Data;
using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext context;

        public CityRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<City> GetAll()
        {
            return context.Cities.ToList();
        }

        public City GetById(int id)
        {
            return context.Cities.FirstOrDefault(c => c.Id == id);
        }

        public void Add(City city)
        {
            context.Cities.Add(city);
        }

        public void Update(City city)
        {
            context.Cities.Update(city);
        }

        public void Delete(int id)
        {
            City city = context.Cities.FirstOrDefault(c => c.Id == id);
            if (city != null)
                context.Cities.Remove(city);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
