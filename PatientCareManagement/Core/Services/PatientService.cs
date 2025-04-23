using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Enums;

namespace PatientCareManagement.Core.Services
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IClinicalAttachmentRepository _attachmentRepository;
        private readonly IBlobStorageService _blobStorageService;

        public PatientService(
            IPatientRepository patientRepository,
            IMedicalHistoryRepository medicalHistoryRepository,
            IClinicalAttachmentRepository attachmentRepository,
            IBlobStorageService blobStorageService)
        {
            _patientRepository = patientRepository;
            _medicalHistoryRepository = medicalHistoryRepository;
            _attachmentRepository = attachmentRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<Patient?> GetPatientAsync(Guid id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return null;
            }

            // Load associated medical histories
            var medicalHistories = await _medicalHistoryRepository.GetByPatientIdAsync(id);
            patient.MedicalHistory = [.. medicalHistories];

            return patient;
        }

        public async Task<MedicalHistory?> GetMedicalHistoryWithAttachmentsAsync(Guid medicalHistoryId)
        {
            var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(medicalHistoryId);
            if (medicalHistory == null)
            {
                return null;
            }

            // Get attachments for this specific medical history
            var attachments = await _attachmentRepository.GetByMedicalHistoryIdAsync(medicalHistoryId);
            medicalHistory.Attachments = [.. attachments];
            
            return medicalHistory;
        }

        public async Task<IEnumerable<Patient?>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Patient?>?> SearchPatientsAsync(string searchTerm, string medicalCondition, AttachmentType? attachmentType)
        {
            // For in memory implementation, get all patients and filter on results
            var patients = await _patientRepository.GetAllAsync();
            if (patients == null) {
                return null;
            }
            
            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                patients = [.. patients.Where(p => 
                    p.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) || 
                    p.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))];
            }

            // Filter by medical condition
            if (!string.IsNullOrWhiteSpace(medicalCondition))
            {
                medicalCondition = medicalCondition.ToLower();
                patients = [.. patients.Where(p => 
                    p.MedicalHistory.Any(mh => 
                        mh.Condition.Contains(medicalCondition, StringComparison.CurrentCultureIgnoreCase) || 
                        mh.Diagnosis.Contains(medicalCondition, StringComparison.CurrentCultureIgnoreCase)))];
            }

            // Filter by attachment type
            if (attachmentType.HasValue)
            {
                patients = [.. patients.Where(p => 
                    p.MedicalHistory.Any(m => 
                        m.Attachments.Any(a => a.AttachmentType == attachmentType.Value)
                    )
                )];
            }

            return patients;
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            return await _patientRepository.AddAsync(patient);
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            await _patientRepository.UpdateAsync(patient);
        }

        public async Task DeletePatientAsync(Guid id)
        {
            // First, get the patient
            var patient = await GetPatientAsync(id);
            // Delete all associated records before deleting the patient record
            if (patient != null)
            {
                // Delete associated medical histories
                foreach (var history in patient.MedicalHistory)
                {
                    // Delete associated blob files
                    foreach (var attachment in history.Attachments)
                    {
                        await _blobStorageService.DeleteFileAsync(attachment.BlobId);
                        await _attachmentRepository.DeleteAsync(attachment.Id);
                    }
                    await _medicalHistoryRepository.DeleteAsync(history.Id);
                }

                // Delete the patient
                await _patientRepository.DeleteAsync(id);
            }
        }

        public async Task<MedicalHistory> AddMedicalHistoryAsync(Guid patientId, MedicalHistory medicalHistory)
        {
            medicalHistory.PatientId = patientId;
            return await _medicalHistoryRepository.AddAsync(medicalHistory);
        }

        public async Task<ClinicalAttachment> UploadAttachmentAsync(
            Guid patientId, 
            Guid? medicalHistoryId, 
            Stream fileStream, 
            string fileName, 
            string contentType, 
            AttachmentType attachmentType, 
            string notes)
        {
            // Verify the medical history exists and belongs to the patient if medicalHistoryId is provided
            if (medicalHistoryId.HasValue)
            {
                var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(medicalHistoryId.Value);
                if (medicalHistory == null || medicalHistory.PatientId != patientId)
                {
                    throw new ArgumentException("Invalid medical history ID or medical history does not belong to the specified patient");
                }
            }

            // Upload file to blob storage
            var blobId = await _blobStorageService.UploadFileAsync(fileStream, fileName, contentType);

            // Create and save attachment metadata
            var attachment = new ClinicalAttachment
            {
                PatientId = patientId,
                MedicalHistoryId = medicalHistoryId,
                FileName = fileName,
                ContentType = contentType,
                BlobId = blobId,
                FileSize = fileStream.Length,
                UploadDate = DateTime.UtcNow,
                AttachmentType = attachmentType,
                Notes = notes
            };

            return await _attachmentRepository.AddAsync(attachment);
        }

        public async Task<Stream?> DownloadAttachmentAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null)
            {
                return null;
            }

            return await _blobStorageService.DownloadFileAsync(attachment.BlobId);
        }

        public async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment != null)
            {
                await _blobStorageService.DeleteFileAsync(attachment.BlobId);
                await _attachmentRepository.DeleteAsync(attachmentId);
            }
        }

        public async Task<IEnumerable<ClinicalAttachment?>> GetAttachmentsByMedicalHistoryAsync(Guid medicalHistoryId)
        {
            return await _attachmentRepository.GetByMedicalHistoryIdAsync(medicalHistoryId);
        }
    }
}