namespace HealthBeside.Application.Contracts;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid RoleId { get; set; } 

    // Поля для PatientProfile
    public DateTime? DateOfBirth { get; set; }
    public string? MedicalHistorySummary { get; set; }

    // Поля для DoctorProfile
    public string? Specialization { get; set; }
    public string? MedicalLicenseNumber { get; set; }
    public string? ClinicAffiliation { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Education { get; set; }
    public string? Biography { get; set; }
    public double? Rating { get; set; }
    
}