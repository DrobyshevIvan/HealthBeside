namespace HealthBeside.Domain.Models;

public class PatientProfile
{
    public Guid Id { get; private set; }
    public string ApplicationUserId { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public string MedicalHistorySummary { get; private set; }

    public ApplicationUser ApplicationUser { get; private set; }

    private PatientProfile()
    {
    }

    public static (string? Error, PatientProfile? PatientProfile) Create(string applicationUserId, DateTime dateOfBirth,
        string address, string phoneNumber, string medicalHistorySummary)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(applicationUserId))
            errors.Add("Application user ID cannot be empty.");

        if (dateOfBirth == default)
            errors.Add("Date of birth is required.");

        if (string.IsNullOrWhiteSpace(address))
            errors.Add("Address cannot be empty.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            errors.Add("Phone number cannot be empty.");

        if (string.IsNullOrWhiteSpace(medicalHistorySummary))
            errors.Add("Medical history summary cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var patientProfile = new PatientProfile
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = applicationUserId,
            DateOfBirth = dateOfBirth,
            Address = address,
            PhoneNumber = phoneNumber,
            MedicalHistorySummary = medicalHistorySummary
        };

        return (null, patientProfile);
    }
}