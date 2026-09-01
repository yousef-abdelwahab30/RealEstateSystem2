using RealEstateSystem.Models;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PropertyTypesController : Controller
    {
        private readonly IPropertyTypeRepository propertyTypeRepository;

        public PropertyTypesController(IPropertyTypeRepository _propertyTypeRepository)
        {
            propertyTypeRepository = _propertyTypeRepository;
        }

        public IActionResult Index()
        {
            List<PropertyType> types = propertyTypeRepository.GetAll();
            return View(types);
        }

        public IActionResult Details(int id)
        {
            PropertyType propertyType = propertyTypeRepository.GetById(id);
            if (propertyType == null)
                return NotFound();

            return View(propertyType);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                propertyTypeRepository.Add(propertyType);
                propertyTypeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(propertyType);
        }

        public IActionResult Edit(int id)
        {
            PropertyType propertyType = propertyTypeRepository.GetById(id);
            if (propertyType == null)
                return NotFound();

            return View(propertyType);
        }

        [HttpPost]
        public IActionResult Edit(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                propertyTypeRepository.Update(propertyType);
                propertyTypeRepository.Save();
                return RedirectToAction("Index");
            }
            return View(propertyType);
        }

        public IActionResult Delete(int id)
        {
            PropertyType propertyType = propertyTypeRepository.GetById(id);
            if (propertyType == null)
                return NotFound();

            return View(propertyType);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            propertyTypeRepository.Delete(id);
            propertyTypeRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
