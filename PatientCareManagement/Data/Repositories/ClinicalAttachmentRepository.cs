using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Enums;
using System.Collections.Concurrent;

namespace PatientCareManagement.Data.Repositories
{
    public class ClinicalAttachmentRepository : IClinicalAttachmentRepository
    {
        private readonly ConcurrentDictionary<Guid, ClinicalAttachment> _attachments = new ConcurrentDictionary<Guid, ClinicalAttachment>();

        public Task<ClinicalAttachment?> GetByIdAsync(Guid id)
        {
            if (_attachments.TryGetValue(id, out var attachment))
            {
                return Task.FromResult<ClinicalAttachment?>(attachment);
            }
            return Task.FromResult<ClinicalAttachment?>(null);
        }

        public Task<IEnumerable<ClinicalAttachment?>> GetByPatientIdAsync(Guid patientId)
        {
            var patientAttachments = _attachments.Values
                .Where(a => a.PatientId == patientId)
                .ToList();
                
            return Task.FromResult<IEnumerable<ClinicalAttachment?>>(patientAttachments);
        }

        public Task<IEnumerable<ClinicalAttachment?>> GetByMedicalHistoryIdAsync(Guid medicalHistoryId)
        {
            var medicalHistoryAttachments = _attachments.Values
                .Where(a => a.MedicalHistoryId == medicalHistoryId)
                .ToList();
                
            return Task.FromResult<IEnumerable<ClinicalAttachment?>>(medicalHistoryAttachments);
        }

        public Task<IEnumerable<ClinicalAttachment>> GetByTypeAsync(AttachmentType attachmentType)
        {
            var typeAttachments = _attachments.Values
                .Where(a => a.AttachmentType == attachmentType)
                .ToList();
                
            return Task.FromResult<IEnumerable<ClinicalAttachment>>(typeAttachments);
        }

        public Task<ClinicalAttachment> AddAsync(ClinicalAttachment attachment)
        {
            if (attachment.Id == Guid.Empty)
            {
                attachment.Id = Guid.NewGuid();
            }
            
            attachment.UploadDate = DateTime.UtcNow;
            _attachments[attachment.Id] = attachment;
            return Task.FromResult(attachment);
        }

        public Task UpdateAsync(ClinicalAttachment attachment)
        {
            _attachments[attachment.Id] = attachment;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _attachments.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}