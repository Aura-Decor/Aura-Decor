namespace AuraDecor.Core.Entities;

public class Address : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? UserId { get; set; }
}