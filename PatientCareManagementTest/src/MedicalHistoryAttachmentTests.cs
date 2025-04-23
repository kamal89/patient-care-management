using Moq;
using PatientCareManagement.Core.Enums;
using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Services;
using Xunit;

namespace PatientCareManagement.Tests
{
    public class MedicalHistoryAttachmentTests
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IMedicalHistoryRepository> _mockMedicalHistoryRepo;
        private readonly Mock<IClinicalAttachmentRepository> _mockAttachmentRepo;
        private readonly Mock<IBlobStorageService> _mockBlobService;
        private readonly PatientService _patientService;

        public MedicalHistoryAttachmentTests()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockMedicalHistoryRepo = new Mock<IMedicalHistoryRepository>();
            _mockAttachmentRepo = new Mock<IClinicalAttachmentRepository>();
            _mockBlobService = new Mock<IBlobStorageService>();

            _patientService = new PatientService(
                _mockPatientRepo.Object,
                _mockMedicalHistoryRepo.Object,
                _mockAttachmentRepo.Object,
                _mockBlobService.Object);
        }

        [Fact]
        public async Task GetMedicalHistoryWithAttachmentsAsync_ShouldReturnMedicalHistoryWithAttachments()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var medicalHistoryId = Guid.NewGuid();
            
            var medicalHistory = new MedicalHistory
            {
                Id = medicalHistoryId,
                PatientId = patientId,
                Condition = "Hypertension",
                Diagnosis = "Stage 1 Hypertension",
                Treatment = "RICE",
                Notes = "",
                DiagnosisDate = DateTime.Now.AddMonths(-6)
            };

            var attachments = new List<ClinicalAttachment>
            {
                new() {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    MedicalHistoryId = medicalHistoryId,
                    FileName = "blood_pressure_readings.pdf",
                    AttachmentType = AttachmentType.LAB_REPORT,
                    ContentType = "pdf",
                    BlobId = "1",
                    Notes = "",
                    UploadDate = DateTime.Now.AddDays(-2)
                },
                new ClinicalAttachment
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    MedicalHistoryId = medicalHistoryId,
                    FileName = "ecg_results.pdf",
                    AttachmentType = AttachmentType.CAT_SCAN,
                    ContentType = "pdf",
                    BlobId = "2",
                    Notes = "",
                    UploadDate = DateTime.Now.AddDays(-1)
                }
            };

            _mockMedicalHistoryRepo.Setup(r => r.GetByIdAsync(medicalHistoryId)).ReturnsAsync(medicalHistory);
            _mockAttachmentRepo.Setup(r => r.GetByMedicalHistoryIdAsync(medicalHistoryId)).ReturnsAsync(attachments);

            // Act
            var result = await _patientService.GetMedicalHistoryWithAttachmentsAsync(medicalHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicalHistoryId, result.Id);
            Assert.Equal("Hypertension", result.Condition);
            Assert.NotNull(result.Attachments);
            Assert.Equal(2, result.Attachments.Count);
            Assert.Equal("blood_pressure_readings.pdf", result.Attachments[0].FileName);
            Assert.Equal("ecg_results.pdf", result.Attachments[1].FileName);
        }

        [Fact]
        public async Task UploadAttachmentAsync_WithMedicalHistoryId_ShouldAssociateAttachmentWithMedicalHistory()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var medicalHistoryId = Guid.NewGuid();
            
            var medicalHistory = new MedicalHistory
            {
                Id = medicalHistoryId,
                PatientId = patientId,
                Condition = "Diabetes",
                Diagnosis = "Too much sugar",
                Treatment = "Insulin",
                Notes = ""
            };

            var fileName = "glucose_test.pdf";
            var contentType = "application/pdf";
            var attachmentType = AttachmentType.LAB_REPORT;
            var notes = "Fasting glucose test results";
            var fileStream = new MemoryStream(new byte[] { 0, 1, 2, 3 });
            var blobId = "test-blob-id";

            _mockMedicalHistoryRepo.Setup(r => r.GetByIdAsync(medicalHistoryId)).ReturnsAsync(medicalHistory);
            _mockBlobService.Setup(r => r.UploadFileAsync(It.IsAny<Stream>(), fileName, contentType)).ReturnsAsync(blobId);
            _mockAttachmentRepo.Setup(r => r.AddAsync(It.IsAny<ClinicalAttachment>()))
                .ReturnsAsync((ClinicalAttachment a) => 
                {
                    a.Id = Guid.NewGuid();
                    return a;
                });

            // Act
            var result = await _patientService.UploadAttachmentAsync(
                patientId,
                medicalHistoryId,
                fileStream,
                fileName,
                contentType,
                attachmentType,
                notes);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.PatientId);
            Assert.Equal(medicalHistoryId, result.MedicalHistoryId);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal(blobId, result.BlobId);
            Assert.Equal(attachmentType, result.AttachmentType);
            Assert.Equal(notes, result.Notes);

            _mockBlobService.Verify(b => b.UploadFileAsync(It.IsAny<Stream>(), fileName, contentType), Times.Once);
            _mockAttachmentRepo.Verify(r => r.AddAsync(It.IsAny<ClinicalAttachment>()), Times.Once);
        }

        [Fact]
        public async Task GetAttachmentsByMedicalHistoryAsync_ShouldReturnAttachmentsForSpecificMedicalHistory()
        {
            // Arrange
            var medicalHistoryId = Guid.NewGuid();
            var attachments = new List<ClinicalAttachment>
            {
                new ClinicalAttachment { Id = Guid.NewGuid(), MedicalHistoryId = medicalHistoryId, FileName = "test1.pdf", BlobId = "3", ContentType = "pdf", Notes = "note" },
                new ClinicalAttachment { Id = Guid.NewGuid(), MedicalHistoryId = medicalHistoryId, FileName = "test2.pdf", BlobId = "4", ContentType = "pdf", Notes = "another note" }
            };

            _mockAttachmentRepo.Setup(r => r.GetByMedicalHistoryIdAsync(medicalHistoryId)).ReturnsAsync(attachments);

            // Act
            var result = await _patientService.GetAttachmentsByMedicalHistoryAsync(medicalHistoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _mockAttachmentRepo.Verify(r => r.GetByMedicalHistoryIdAsync(medicalHistoryId), Times.Once);
        }
    }
}