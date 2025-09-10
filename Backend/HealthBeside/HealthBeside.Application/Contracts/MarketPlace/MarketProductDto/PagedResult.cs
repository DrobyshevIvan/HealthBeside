namespace HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;

public class PagedResult
{
    public IEnumerable<GetMarketProductDto>  Products { get; set; }
    public int Total { get; set; }
}