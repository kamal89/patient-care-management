using System;

namespace PatientCareManagement.Core.Models
{
    public class MedicalHistory
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public required string Condition { get; set; }
        public required string Diagnosis { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public required string Treatment { get; set; }
        public required string Notes { get; set; }
        public List<ClinicalAttachment> Attachments { get; set; } = new List<ClinicalAttachment>();
    }
}