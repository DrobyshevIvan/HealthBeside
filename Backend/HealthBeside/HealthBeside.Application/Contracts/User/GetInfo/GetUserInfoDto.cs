using HealthBeside.Application.Contracts.User;

namespace HealthBeside.Application.Contracts;

public class GetUserInfoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
}