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
        var errors = new List<string>();
        
        if(string.IsNullOrWhiteSpace(city))
            errors.Add("City cannot be empty.");
        
        if(string.IsNullOrWhiteSpace(phoneNumber))
            errors.Add("Phone number cannot be empty.");
        
        if(postalIndex <= 0)
            errors.Add("Postal index must be a positive number.");
        
        if(string.IsNullOrWhiteSpace(streetName))
            errors.Add("Street name cannot be empty.");
        
        if(streetNumber <= 0)
            errors.Add("Street number must be a positive number.");
        
        if(errors.Any())
            return (string.Join("; ", errors), null);
        
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

    public string? Update(
        string? city,
        string? phoneNumber,
        int? postalIndex,
        string? streetName,
        int? streetNumber)
    {
        bool hasChanges = false;
        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(city) && city != City)
        {
            City = city;
            hasChanges = true;
        }
        else if (city is not null && string.IsNullOrWhiteSpace(city))
        {
            errors.Add("City cannot be empty.");
        }

        if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber != PhoneNumber)
        {
            PhoneNumber = phoneNumber;
            hasChanges = true;
        }
        else if (phoneNumber is not null && string.IsNullOrWhiteSpace(phoneNumber))
        {
            errors.Add("Phone number cannot be empty.");
        }

        if (postalIndex.HasValue)
        {
            if (postalIndex.Value <= 0)
            {
                errors.Add("Postal index must be a positive number.");
            }
            else if (postalIndex.Value != PostalIndex)
            {
                PostalIndex = postalIndex.Value;
                hasChanges = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(streetName) && streetName != StreetName)
        {
            StreetName = streetName;
            hasChanges = true;
        }
        else if (streetName is not null && string.IsNullOrWhiteSpace(streetName))
        {
            errors.Add("Street name cannot be empty.");
        }

        if (streetNumber.HasValue)
        {
            if (streetNumber.Value <= 0)
            {
                errors.Add("Street number must be a positive number.");
            }
            else if (streetNumber.Value != StreetNumber)
            {
                StreetNumber = streetNumber.Value;
                hasChanges = true;
            }
        }
        
        if (errors.Any())
            return string.Join("; ", errors);
        
        return null;
    }

}