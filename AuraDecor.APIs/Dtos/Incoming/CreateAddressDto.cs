using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class CreateAddressDto
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }
        
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        
        [Required(ErrorMessage = "Zip Code is required")]
        public string ZipCode { get; set; }
        
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
        
        // Optional
        public string? PhoneNumber { get; set; }
        
        // Optional - additional delivery instructions
        public string? DeliveryInstructions { get; set; }
    }
}
