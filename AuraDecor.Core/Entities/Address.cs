namespace AuraDecor.Core.Entities;

public class Address : BaseEntity
{
    public string FName { get; set; }
    public string LName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string UserId { get; set; }
    public string Country { get; set; }
}