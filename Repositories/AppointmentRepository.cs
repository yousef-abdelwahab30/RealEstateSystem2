using RealEstateSystem.Data;
using RealEstateSystem.Models;
using RealEstateSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace RealEstateSystem.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext context;

        public AppointmentRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public List<Appointment> GetAll()
        {
            return context.Appointments
                .Include(a => a.Property)
                .ToList();
        }

        public Appointment GetById(int id)
        {
            return context.Appointments
                .Include(a => a.Property)
                .FirstOrDefault(a => a.Id == id);
        }

        public void Add(Appointment appointment)
        {
            context.Appointments.Add(appointment);
        }

        public void Update(Appointment appointment)
        {
            context.Appointments.Update(appointment);
        }

        public void Delete(int id)
        {
            Appointment appointment = context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
                context.Appointments.Remove(appointment);
        }

        public void Confirm(int id)
        {
            Appointment appointment = context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
                appointment.Status = AppointmentStatus.Confirmed;
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
