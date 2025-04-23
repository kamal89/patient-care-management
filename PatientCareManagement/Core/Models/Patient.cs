using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientCareManagement.Core.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required ContactDetails ContactDetails { get; set; }
        public List<MedicalHistory> MedicalHistory { get; set; } = new List<MedicalHistory>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Helper properties
        public int Age => CalculateAge(DateOfBirth);
        public string FullName => $"{FirstName} {LastName}";

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
