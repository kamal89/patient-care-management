using PatientCareManagement.Core.Models;

namespace PatientCareManagement.Core.Interfaces
{
    public interface IMedicalHistoryRepository
    {
        Task<MedicalHistory?> GetByIdAsync(Guid id);
        Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId);
        Task<MedicalHistory> AddAsync(MedicalHistory medicalHistory);
        Task UpdateAsync(MedicalHistory medicalHistory);
        Task DeleteAsync(Guid id);
    }
}