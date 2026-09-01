using RealEstateSystem.Models;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CitiesController : Controller
    {
        private readonly ICityRepository cityRepository;

        public CitiesController(ICityRepository _cityRepository)
        {
            cityRepository = _cityRepository;
        }

        public IActionResult Index()
        {
            List<City> cities = cityRepository.GetAll();
            return View(cities);
        }

        public IActionResult Details(int id)
        {
            City city = cityRepository.GetById(id);
            if (city == null)
                return NotFound();

            return View(city);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(City city)
        {
            if (ModelState.IsValid)
            {
                cityRepository.Add(city);
                cityRepository.Save();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        public IActionResult Edit(int id)
        {
            City city = cityRepository.GetById(id);
            if (city == null)
                return NotFound();

            return View(city);
        }

        [HttpPost]
        public IActionResult Edit(City city)
        {
            if (ModelState.IsValid)
            {
                cityRepository.Update(city);
                cityRepository.Save();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        public IActionResult Delete(int id)
        {
            City city = cityRepository.GetById(id);
            if (city == null)
                return NotFound();

            return View(city);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            cityRepository.Delete(id);
            cityRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
