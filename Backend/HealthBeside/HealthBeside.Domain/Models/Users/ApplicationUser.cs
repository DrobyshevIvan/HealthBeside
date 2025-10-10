using System.ComponentModel.DataAnnotations;
using HealthBeside.Domain.Models.Forum;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Domain.Models.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    // Navigation properties for related profiles
    public Guid DoctorProfileId { get; set; }
    public DoctorProfile DoctorProfile { get; set; }
    public Guid PatientProfileId { get; set; }
    public PatientProfile PatientProfile { get; set; }
    
    //Address info
    public UserDeliveryInfo UserDeliveryInfo { get; private set; }

    // Forum
    public ICollection<ForumPost> ForumPosts { get; private set; }
    public ICollection<ForumComment> ForumComments { get; private set; }

    // Marketplace
    public ICollection<MarketReview> MarketReviews { get; private set; }
    public MarketCart? MarketCart { get; private set; }
    public ICollection<MarketOrder> MarketOrders { get; private set; }

    // Auth
    public ICollection<RefreshToken> RefreshToken { get; private set; }
    public ICollection<Payment> Payments { get; private set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; private set; }
    
    

    public ApplicationUser() { }

    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
    
    public static (string? Error, ApplicationUser ApplicationUser) Create(
        string firstName,
        string lastName,
        string email)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add("Last name cannot be empty.");

        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            errors.Add("Invalid email address.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var applicationUser = new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            RegistrationDate = DateTime.UtcNow
        };

        return (null, applicationUser);
    }

    public string Update(string firstName, string lastName, DateTime dateOfBirth)
    {
        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(firstName) && firstName != FirstName)
        {
            FirstName = firstName;
        }
        else if (string.IsNullOrWhiteSpace(firstName))
        {
            errors.Add("First name cannot be empty.");
        }

        if (!string.IsNullOrWhiteSpace(lastName) && lastName != LastName)
        {
            LastName = lastName;
        }
        else if (string.IsNullOrWhiteSpace(lastName))
        {
            errors.Add("Last name cannot be empty.");
        }

        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }

    public string UpdatePatientProfile(Guid patientProfileId)
    {
        var errors = new List<string>();

        if (!patientProfileId.Equals(Guid.Empty))
        {
            PatientProfileId = patientProfileId;
        }
        else
        {
            errors.Add("Patient profile id is empty");
        }

        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }
    
    public string UpdateDoctorProfile(Guid doctorProfileId)
    {
        var errors = new List<string>();

        if (!doctorProfileId.Equals(Guid.Empty))
        {
            DoctorProfileId = doctorProfileId;
        }
        else
        {
            errors.Add("Doctor profile id is empty");
        }

        if (errors.Any())
            return string.Join("; ", errors);

        return null;
    }
}

