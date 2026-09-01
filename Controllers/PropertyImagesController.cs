using RealEstateSystem.Models;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateSystem.Controllers
{
    [Authorize(Roles = "Admin,Agent")]
    public class PropertyImagesController : Controller
    {
        private readonly IPropertyImageRepository propertyImageRepository;
        private readonly IPropertyRepository propertyRepository;

        public PropertyImagesController(
            IPropertyImageRepository _propertyImageRepository,
            IPropertyRepository _propertyRepository)
        {
            propertyImageRepository = _propertyImageRepository;
            propertyRepository = _propertyRepository;
        }

        public IActionResult Index()
        {
            List<PropertyImage> images = propertyImageRepository.GetAll();
            return View(images);
        }

        public IActionResult Details(int id)
        {
            PropertyImage propertyImage = propertyImageRepository.GetById(id);
            if (propertyImage == null)
                return NotFound();

            return View(propertyImage);
        }

        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(PropertyImage propertyImage)
        {
            if (ModelState.IsValid)
            {
                propertyImageRepository.Add(propertyImage);
                propertyImageRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(propertyImage);
            return View(propertyImage);
        }

        public IActionResult Edit(int id)
        {
            PropertyImage propertyImage = propertyImageRepository.GetById(id);
            if (propertyImage == null)
                return NotFound();

            PopulateDropDowns(propertyImage);
            return View(propertyImage);
        }

        [HttpPost]
        public IActionResult Edit(PropertyImage propertyImage)
        {
            if (ModelState.IsValid)
            {
                propertyImageRepository.Update(propertyImage);
                propertyImageRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(propertyImage);
            return View(propertyImage);
        }

        public IActionResult Delete(int id)
        {
            PropertyImage propertyImage = propertyImageRepository.GetById(id);
            if (propertyImage == null)
                return NotFound();

            return View(propertyImage);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            propertyImageRepository.Delete(id);
            propertyImageRepository.Save();
            return RedirectToAction("Index");
        }

        private void PopulateDropDowns(PropertyImage propertyImage = null)
        {
            ViewBag.PropertyId = new SelectList(propertyRepository.GetAll(), "Id", "Title", propertyImage?.PropertyId);
        }
    }
}
