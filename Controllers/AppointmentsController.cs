using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateSystem.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IPropertyRepository propertyRepository;

        public AppointmentsController(
            IAppointmentRepository _appointmentRepository,
            IPropertyRepository _propertyRepository)
        {
            appointmentRepository = _appointmentRepository;
            propertyRepository = _propertyRepository;
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Index()
        {
            List<Appointment> appointments = appointmentRepository.GetAll();
            return View(appointments);
        }

        public IActionResult Details(int id)
        {
            Appointment appointment = appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedAt = DateTime.Now;
                appointment.Status = AppointmentStatus.Pending;

                appointmentRepository.Add(appointment);
                appointmentRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(appointment);
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Edit(int id)
        {
            Appointment appointment = appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();

            PopulateDropDowns(appointment);
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Edit(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointmentRepository.Update(appointment);
                appointmentRepository.Save();
                return RedirectToAction("Index");
            }

            PopulateDropDowns(appointment);
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Delete(int id)
        {
            Appointment appointment = appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult DeleteConfirmed(int id)
        {
            appointmentRepository.Delete(id);
            appointmentRepository.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult Confirm(int id)
        {
            appointmentRepository.Confirm(id);
            appointmentRepository.Save();
            return RedirectToAction("Index");
        }

        private void PopulateDropDowns(Appointment appointment = null)
        {
            ViewBag.PropertyId = new SelectList(propertyRepository.GetAll(), "Id", "Title", appointment?.PropertyId);
        }
    }
}
