namespace HealthBeside.Domain.Models;

public class DoctorProfile
{
    public Guid Id { get; private set; }
    public string ApplicationUserId { get; private set; }
    public string Specialization { get; private set; }
    public string MedicalLicenseNumber { get; private set; }
    public string ClinicAffiliation { get; private set; }
    public int YearsOfExperience { get; private set; }
    public string Education { get; private set; }
    public string Biography { get; private set; }
    public double Rating { get; private set; }
    
    public ApplicationUser ApplicationUser { get; private set; }
    
    private DoctorProfile() {}
    
    public static (string? Error, DoctorProfile? DoctorProfile) Create(
        string applicationUserId,
        string specialization,
        string medicalLicenseNumber,
        string clinicAffiliation,
        int yearsOfExperience,
        string education,
        string biography,
        double rating)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(applicationUserId))
            errors.Add("Application user ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(specialization))
            errors.Add("Specialization cannot be empty.");

        if (string.IsNullOrWhiteSpace(medicalLicenseNumber))
            errors.Add("Medical license number cannot be empty.");

        if (string.IsNullOrWhiteSpace(clinicAffiliation))
            errors.Add("Clinic affiliation cannot be empty.");

        if (yearsOfExperience < 0)
            errors.Add("Years of experience cannot be negative.");

        if (string.IsNullOrWhiteSpace(education))
            errors.Add("Education cannot be empty.");

        if (string.IsNullOrWhiteSpace(biography))
            errors.Add("Biography cannot be empty.");

        if (rating < 0 || rating > 5)
            errors.Add("Rating must be between 0 and 5.");

        if (errors.Any())
            return (string.Join("; ", errors), null);

        var doctorProfile = new DoctorProfile
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = applicationUserId,
            Specialization = specialization,
            MedicalLicenseNumber = medicalLicenseNumber,
            ClinicAffiliation = clinicAffiliation,
            YearsOfExperience = yearsOfExperience,
            Education = education,
            Biography = biography,
            Rating = rating
        };

        return (null, doctorProfile);
    }
}