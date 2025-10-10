namespace HealthBeside.Application.Contracts.User;

public class RegisterPatientProfileRequest
{
    // Поля для PatientProfile
    public DateTime? DateOfBirth { get; set; }
    public string? MedicalHistorySummary { get; set; }
}