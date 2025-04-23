using Moq;
using PatientCareManagement.Core.Services;
using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Interfaces;

namespace PatientCareManagementTest.src
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _mockRepository;
        private readonly Mock<IMedicalHistoryRepository> _mockMedicalHistoryRepository;
        private readonly Mock<IClinicalAttachmentRepository> _mockClinicalAttachmentRepository;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly PatientService _patientService;

        public PatientServiceTests()
        {
            _mockRepository = new Mock<IPatientRepository>();
            _mockMedicalHistoryRepository = new Mock<IMedicalHistoryRepository>();
            _mockClinicalAttachmentRepository = new Mock<IClinicalAttachmentRepository>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _patientService = new PatientService(
                _mockRepository.Object,
                _mockMedicalHistoryRepository.Object,
                _mockClinicalAttachmentRepository.Object,
                _mockBlobStorageService.Object
            );
        }

        [Fact]
        public async Task AddPatient_ShouldAddPatient_WhenValidPatientIsProvidedAsync()
        {
            // Arrange
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                ContactDetails = new ContactDetails 
                { 
                    Phone = "123-456-7890", 
                    Email = "email", 
                    Address = new Address { Street = "123 Main St", City = "Sample City", State = "Sample State", ZipCode = "12345", Country = "Sample Country" } 
                },
            };
            _mockRepository.Setup(repo => repo.AddAsync(patient)).Returns(Task.FromResult(patient));

            // Act
            var result = await _patientService.CreatePatientAsync(patient);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(repo => repo.AddAsync(patient), Times.Once);
        }

        [Fact]
        public async Task GetPatientById_ShouldReturnPatient_WhenPatientExists()
        {
            // Arrange
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                ContactDetails = new ContactDetails
                {
                    Phone = "123-456-7890",
                    Email = "john.doe@example.com",
                    Address = new Address
                    {
                        Street = "123 Main St",
                        City = "Sample City",
                        State = "Sample State",
                        ZipCode = "12345",
                        Country = "Sample Country"
                    }
                }
            };
            var patientId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(patientId)).ReturnsAsync(patient);

            // Act
            _mockRepository.Setup(repo => repo.GetByIdAsync(patientId)).ReturnsAsync(patient);
            var result = _patientService.GetPatientAsync(patientId);

            // Assert
            Assert.NotNull(result);
            var patientResult = await result;
            Assert.Equal("John Doe", patientResult.FullName);
            _mockRepository.Verify(repo => repo.GetByIdAsync(patientId), Times.Once);
        }
    }
}