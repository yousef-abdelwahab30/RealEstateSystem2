using RealEstateSystem.Models;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AgentsController : Controller
    {
        private readonly IAgentRepository agentRepository;

        public AgentsController(IAgentRepository _agentRepository)
        {
            agentRepository = _agentRepository;
        }

        public IActionResult Index()
        {
            List<Agent> agents = agentRepository.GetAll();
            return View(agents);
        }

        public IActionResult Details(int id)
        {
            Agent agent = agentRepository.GetById(id);
            if (agent == null)
                return NotFound();

            return View(agent);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Agent agent)
        {
            if (ModelState.IsValid)
            {
                agentRepository.Add(agent);
                agentRepository.Save();
                return RedirectToAction("Index");
            }
            return View(agent);
        }

        public IActionResult Edit(int id)
        {
            Agent agent = agentRepository.GetById(id);
            if (agent == null)
                return NotFound();

            return View(agent);
        }

        [HttpPost]
        public IActionResult Edit(Agent agent)
        {
            if (ModelState.IsValid)
            {
                agentRepository.Update(agent);
                agentRepository.Save();
                return RedirectToAction("Index");
            }
            return View(agent);
        }

        public IActionResult Delete(int id)
        {
            Agent agent = agentRepository.GetById(id);
            if (agent == null)
                return NotFound();

            return View(agent);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            agentRepository.Delete(id);
            agentRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
