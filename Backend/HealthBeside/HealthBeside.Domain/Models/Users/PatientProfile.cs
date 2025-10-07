namespace HealthBeside.Domain.Models.Users;

public class PatientProfile
{
    public Guid Id { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string MedicalHistorySummary { get; private set; } // TODO
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser ApplicationUser { get; private set; }

    private PatientProfile() { }

    public static (string? Error, PatientProfile? PatientProfile) Create(
        Guid applicationUserId,
        DateTime dateOfBirth,
        string medicalHistorySummary,
        ApplicationUser user)
    {
        var errors = new List<string>();

        if (applicationUserId == Guid.Empty)
            errors.Add("Application user ID cannot be empty.");
        
        if (dateOfBirth == default)
            errors.Add("Date of birth is required.");

        if (user == null)
            errors.Add("User cannot be null.");

        if (string.IsNullOrWhiteSpace(medicalHistorySummary))
            errors.Add("Medical history summary cannot be empty.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var patientProfile = new PatientProfile
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = applicationUserId,
            DateOfBirth = dateOfBirth,
            MedicalHistorySummary = medicalHistorySummary
        };

        return (null, patientProfile);
    }

    public string? Update(string medicalHistorySummary)
    {
        var errors = new List<string>();
        bool hasChanges = false;

        if (!string.IsNullOrWhiteSpace(medicalHistorySummary) && medicalHistorySummary != MedicalHistorySummary)
        {
            MedicalHistorySummary = medicalHistorySummary;
            hasChanges = true;
        }

        if (!hasChanges)
            errors.Add("No changes detected.");

        return errors.Any() ? string.Join("; ", errors) : null;
    }
}