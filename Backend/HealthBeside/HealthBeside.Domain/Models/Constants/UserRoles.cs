namespace HealthBeside.Domain.Models.Constants;

public class UserRoles
{
    public static readonly Guid AdminRoleId = Guid.Parse("");
    public static readonly Guid UserRoleId = Guid.Parse("");
    public static readonly Guid DoctorRoleId = Guid.Parse("");
    public static readonly Guid PatientRoleId = Guid.Parse("");
    
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