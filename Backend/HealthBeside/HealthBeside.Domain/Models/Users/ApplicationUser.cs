using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HealthBeside.Domain.Models.Users;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    
    // Navigation properties for related profiles
    public DoctorProfile DoctorProfile { get; private set; }
    public PatientProfile PatientProfile { get; private set; }

    //Зробити ще навігаційні властивості форуму, замовлення, коментарів на форумі і наче все 
    
    private ApplicationUser() { }
    
    public static (string? Error, ApplicationUser ApplicationUser) Create(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add("Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            errors.Add("Invalid email address.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            errors.Add("Password must be at least 6 characters long.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var applicationUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            RegistrationDate = DateTime.UtcNow
        };

        return (null, applicationUser);
    }
    
}