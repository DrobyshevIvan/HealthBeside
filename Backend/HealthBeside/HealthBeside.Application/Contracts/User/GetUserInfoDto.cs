namespace HealthBeside.Application.Contracts;

public class GetUserInfoDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
}