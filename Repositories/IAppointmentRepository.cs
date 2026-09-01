using RealEstateSystem.Models;

namespace RealEstateSystem.Repositories
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAll();
        Appointment GetById(int id);
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(int id);
        void Confirm(int id);
        void Save();
    }
}
