namespace HealthBeside.Application.Contracts.User;

public class UpdatePatientProfileRequest : UpdateUserRequest
{
    public string? MedicalHistorySummary { get; set; }
}