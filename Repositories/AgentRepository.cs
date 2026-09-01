using RealEstateSystem.Data;
using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly ApplicationDbContext context;

        public AgentRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<Agent> GetAll()
        {
            return context.Agents.ToList();
        }

        public Agent GetById(int id)
        {
            return context.Agents.FirstOrDefault(a => a.Id == id);
        }

        public void Add(Agent agent)
        {
            context.Agents.Add(agent);
        }

        public void Update(Agent agent)
        {
            context.Agents.Update(agent);
        }

        public void Delete(int id)
        {
            Agent agent = context.Agents.FirstOrDefault(a => a.Id == id);
            if (agent != null)
                context.Agents.Remove(agent);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
