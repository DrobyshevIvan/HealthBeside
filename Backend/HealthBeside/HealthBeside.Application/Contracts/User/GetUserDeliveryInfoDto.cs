namespace HealthBeside.Application.Contracts.User;

public class GetUserDeliveryInfoDto
{
    public Guid Id { get; set; }
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public int PostalIndex { get; set; }
    public string StreetName { get; set; }
    public int StreetNumber { get; set; }
}