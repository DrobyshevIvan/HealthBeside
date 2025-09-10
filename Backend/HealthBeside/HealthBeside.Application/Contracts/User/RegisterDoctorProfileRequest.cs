namespace HealthBeside.Application.Contracts.User;

public class RegisterDoctorProfileRequest : RegisterRequestBase
{
    // Поля для DoctorProfile
    public string? Specialization { get; set; }
    public string? MedicalLicenseNumber { get; set; }
    public string? ClinicAffiliation { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Education { get; set; }
    public string? Biography { get; set; }
}