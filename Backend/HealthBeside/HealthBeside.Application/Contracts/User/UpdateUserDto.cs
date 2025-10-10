namespace HealthBeside.Application.Contracts.User;

public class UpdateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}