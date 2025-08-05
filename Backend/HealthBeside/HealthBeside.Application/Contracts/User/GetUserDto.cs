namespace HealthBeside.Application.Contracts;

public class GetUserDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}