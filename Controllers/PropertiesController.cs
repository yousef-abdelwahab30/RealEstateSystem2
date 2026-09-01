using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateSystem.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IPropertyTypeRepository propertyTypeRepository;
        private readonly ICityRepository cityRepository;
        private readonly IAgentRepository agentRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PropertiesController(
            IPropertyRepository _propertyRepository,
            IPropertyTypeRepository _propertyTypeRepository,
            ICityRepository _cityRepository,
            IAgentRepository _agentRepository,
            IWebHostEnvironment _webHostEnvironment)
        {
            propertyRepository = _propertyRepository;
            propertyTypeRepository = _propertyTypeRepository;
            cityRepository = _cityRepository;
            agentRepository = _agentRepository;
            webHostEnvironment = _webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Property> properties = propertyRepository.GetAll();
            return View(properties);
        }

        public IActionResult Search(int? cityId, int? propertyTypeId, ListingType? listingType,
            decimal? minPrice, decimal? maxPrice, int? bedrooms)
        {
            List<Property> results = propertyRepository.Search(
                cityId, propertyTypeId, listingType, minPrice, maxPrice, bedrooms);

            PopulateDropDowns();
            return View(results);
        }

        public IActionResult Details(int id)
        {
            Property property = propertyRepository.GetById(id);
            if (property == null)
                return NotFound();

            return View(property);
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Create(Property property)
        {
            if (ModelState.IsValid)
            {
                property.CreatedAt = DateTime.Now;
                property.Status = PropertyStatus.Pending;

                if (property.ImageFile != null)
                    property.MainImageUrl = SaveImage(property.ImageFile);

                propertyRepository.Add(property);
                propertyRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(property);
            return View(property);
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Edit(int id)
        {
            Property property = propertyRepository.GetById(id);
            if (property == null)
                return NotFound();

            PopulateDropDowns(property);
            return View(property);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Edit(Property property)
        {
            if (ModelState.IsValid)
            {
                if (property.ImageFile != null)
                    property.MainImageUrl = SaveImage(property.ImageFile);

                propertyRepository.Update(property);
                propertyRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(property);
            return View(property);
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Delete(int id)
        {
            Property property = propertyRepository.GetById(id);
            if (property == null)
                return NotFound();

            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult DeleteConfirmed(int id)
        {
            propertyRepository.Delete(id);
            propertyRepository.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            propertyRepository.Approve(id);
            propertyRepository.Save();
            return RedirectToAction("Index");
        }

        private string SaveImage(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string fullPath = Path.Combine(uploadsFolder, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return "/uploads/" + fileName;
        }

        private void PopulateDropDowns(Property property = null)
        {
            ViewBag.PropertyTypeId = new SelectList(propertyTypeRepository.GetAll(), "Id", "Name", property?.PropertyTypeId);
            ViewBag.CityId = new SelectList(cityRepository.GetAll(), "Id", "Name", property?.CityId);
            ViewBag.AgentId = new SelectList(agentRepository.GetAll(), "Id", "FullName", property?.AgentId);
        }
    }
}
