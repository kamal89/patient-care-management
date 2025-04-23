using System.Collections.Concurrent;
using PatientCareManagement.Core.Enums;
using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Models;

namespace PatientCareManagement.Data.Repositories
{
    /// <summary>
    /// In-memory implementation of IPatientRepository for development and testing.
    /// In a production environment, this would be replaced with a real database implementation.
    /// </summary>
    public class PatientRepository : IPatientRepository
    {
        private readonly ConcurrentDictionary<Guid, Patient> _patients = new ConcurrentDictionary<Guid, Patient>();

        public Task<Patient?> GetByIdAsync(Guid id)
        {
            if (_patients.TryGetValue(id, out var patient))
            {
                return Task.FromResult<Patient?>(patient);
            }
            return Task.FromResult<Patient?>(null);
        }

        public Task<IEnumerable<Patient?>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Patient?>>([.. _patients.Values]);
        }

        public Task<IEnumerable<Patient?>> SearchAsync(string searchTerm, string medicalCondition, AttachmentType? attachmentType)
        {
            var results = _patients.Values.AsEnumerable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                results = results.Where(p => 
                    p.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) || 
                    p.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            // Filter by medical condition
            if (!string.IsNullOrWhiteSpace(medicalCondition))
            {
                medicalCondition = medicalCondition.ToLower();
                results = results.Where(p => 
                    p.MedicalHistory.Any(mh => 
                        mh.Condition.Contains(medicalCondition, StringComparison.CurrentCultureIgnoreCase) || 
                        mh.Diagnosis.Contains(medicalCondition, StringComparison.CurrentCultureIgnoreCase)));
            }

            // Filter by attachment type
            if (attachmentType.HasValue)
            {
                results = [.. results.Where(p => 
                    p.MedicalHistory.Any(m => 
                        m.Attachments.Any(a => a.AttachmentType == attachmentType.Value)
                    )
                )];
            }

            return Task.FromResult<IEnumerable<Patient?>>(results);
        }

        public Task<Patient> AddAsync(Patient patient)
        {
            if (patient.Id == Guid.Empty)
            {
                patient.Id = Guid.NewGuid();
            }
            
            _patients[patient.Id] = patient;
            return Task.FromResult(patient);
        }

        public Task UpdateAsync(Patient patient)
        {
            _patients[patient.Id] = patient;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _patients.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}