using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;

namespace HealthBeside.Application.Interfaces;

public interface IMarketProductService
{
    Task<IEnumerable<GetMarketProductDto>> GetAllAsync();
    Task<GetDetailedMarketProductDto> GetByIdAsync(Guid id);
    Task<GetDetailedMarketProductDto> CreateAsync(CreateMarketProductDto createDto);
    Task<bool> UpdateAsync(Guid id, UpdateMarketProductDto updateMarketProductDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}