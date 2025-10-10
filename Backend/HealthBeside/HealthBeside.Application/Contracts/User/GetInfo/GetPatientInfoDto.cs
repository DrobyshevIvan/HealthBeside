using System.Text.Json.Serialization;
using HealthBeside.Infrastructure;

namespace HealthBeside.Application.Contracts.User;

public class GetPatientInfoDto : GetUserInfoDto
{
    [JsonConverter(typeof(DateTimeLocalJsonConverter))]
    public DateTime DateOfBirth { get; set; }
    public string MedicalHistorySummary { get; set; }
}