using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public interface IAgentRepository
    {
        List<Agent> GetAll();
        Agent GetById(int id);
        void Add(Agent agent);
        void Update(Agent agent);
        void Delete(int id);
        void Save();
    }
}
