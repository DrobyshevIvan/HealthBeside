namespace HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;

public class GetUserDeliveryInfoDto
{
    public string City { get; set; }
    public string PhoneNumber { get; set; }
    public int PostalIndex { get; set; }
    public string StreetName { get; set; }
    public int StreetNumber { get; set; }
}
