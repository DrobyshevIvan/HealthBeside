namespace HealthBeside.Application.Contracts.User;

public class RegisterPatientProfile : RegisterRequestBase
{
    // Поля для PatientProfile
    public DateTime? DateOfBirth { get; set; }
    public string? MedicalHistorySummary { get; set; }
}