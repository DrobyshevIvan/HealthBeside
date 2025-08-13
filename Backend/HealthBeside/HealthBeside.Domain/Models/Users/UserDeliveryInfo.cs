using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Domain.Models.Users;

public class UserDeliveryInfo
{
    public Guid Id { get; private set; }
    public string City { get; private set; }
    public string PhoneNumber { get; private set; }
    public int PostalIndex { get; private set; }
    public string StreetName { get; private set; }
    public int StreetNumber { get; private set; }
    
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser ApplicationUser { get; private set; }
    
    private UserDeliveryInfo() { }

    public static (string? Error, UserDeliveryInfo? UserDeliveryInfo) Create(
        string city,
        string phoneNumber, 
        int postalIndex, 
        string streetName, 
        int streetNumber,
        Guid applicationUserId)
    {
        var deliveryInfo =  new UserDeliveryInfo
        {
            Id = Guid.NewGuid(),
            City = city,
            PhoneNumber = phoneNumber,
            PostalIndex = postalIndex,
            StreetName = streetName,
            StreetNumber = streetNumber,
            ApplicationUserId = applicationUserId
        };

        return (null, deliveryInfo);
    }
        
}