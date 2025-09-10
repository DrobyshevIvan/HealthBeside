namespace HealthBeside.Domain.Models.Constants;

public class UserRoles
{
    public static readonly Guid AdminRoleId = Guid.Parse("aeec2a74-61cd-41eb-8ac7-251c365da8c5");
    public static readonly Guid UserRoleId = Guid.Parse("098f240f-01cf-4925-ba10-4983724f73c3");
    public static readonly Guid DoctorRoleId = Guid.Parse("665c40a7-46ec-4564-a679-755f77c90472");
    public static readonly Guid PatientRoleId = Guid.Parse("c1167d0e-ff05-4df7-bfb5-69baf52c174f");
    
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";

    public static Dictionary<Guid, string> RoleMapping = new()
    {
        { AdminRoleId, Admin },
        { UserRoleId, User },
        { DoctorRoleId, Doctor },
        { PatientRoleId, Patient },
    };

}