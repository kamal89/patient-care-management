using PatientCareManagement.Core.Enums;

namespace PatientCareManagement.Core.Models
{
    public class ClinicalAttachment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? MedicalHistoryId { get; set; }  // Reference to specific medical condition
        public required string FileName { get; set; }
        public required string ContentType { get; set; }
        public required string BlobId { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public AttachmentType AttachmentType { get; set; }
        public required string Notes { get; set; }
    }
}