using PatientCareManagement.Core.Models;

namespace PatientCareManagement.Core.Interfaces
{
    public interface IClinicalAttachmentRepository
    {
        Task<ClinicalAttachment?> GetByIdAsync(Guid id);
        Task<IEnumerable<ClinicalAttachment?>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<ClinicalAttachment?>> GetByMedicalHistoryIdAsync(Guid medicalHistoryId);
        Task<ClinicalAttachment> AddAsync(ClinicalAttachment attachment);
        Task UpdateAsync(ClinicalAttachment attachment);
        Task DeleteAsync(Guid id);
    }
}