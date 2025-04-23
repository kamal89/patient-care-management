using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Enums;

namespace PatientCareManagement.Core.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient?>> GetAllAsync();
        Task<IEnumerable<Patient?>> SearchAsync(string searchTerm, string medicalCondition, AttachmentType? attachmentType);
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Guid id);
    }
}