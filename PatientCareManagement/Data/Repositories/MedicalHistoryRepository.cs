using PatientCareManagement.Core.Interfaces;
using PatientCareManagement.Core.Models;
using System.Collections.Concurrent;

namespace PatientCareManagement.Data.Repositories
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly ConcurrentDictionary<Guid, MedicalHistory> _medicalHistories = new ConcurrentDictionary<Guid, MedicalHistory>();

        public Task<MedicalHistory?> GetByIdAsync(Guid id)
        {
            if (_medicalHistories.TryGetValue(id, out var history))
            {
                return Task.FromResult<MedicalHistory?>(history);
            }
            return Task.FromResult<MedicalHistory?>(null);
        }

        public Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId)
        {
            var patientHistories = _medicalHistories.Values
                .Where(mh => mh.PatientId == patientId)
                .ToList();
                
            return Task.FromResult<IEnumerable<MedicalHistory>>(patientHistories);
        }

        public Task<MedicalHistory> AddAsync(MedicalHistory medicalHistory)
        {
            if (medicalHistory.Id == Guid.Empty)
            {
                medicalHistory.Id = Guid.NewGuid();
            }
            
            _medicalHistories[medicalHistory.Id] = medicalHistory;
            return Task.FromResult(medicalHistory);
        }

        public Task UpdateAsync(MedicalHistory medicalHistory)
        {
            _medicalHistories[medicalHistory.Id] = medicalHistory;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _medicalHistories.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}