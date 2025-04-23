
namespace PatientCareManagement.Core.Models
{
    public class ContactDetails
    {
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required Address Address { get; set; }
    }
    
    public class Address
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
    }
}