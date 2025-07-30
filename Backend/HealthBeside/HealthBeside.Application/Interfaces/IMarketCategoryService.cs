using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IMarketCategoryService
{
    Task<IEnumerable<GetMarketCategoryDto>> GetAllAsync();
    Task<GetMarketCategoryDto> GetByIdAsync(Guid id);
    Task<GetMarketCategoryDto> CreateAsync(CreateMarketCategoryDto createMarketCategoryDto);
    Task<bool> UpdateAsync(Guid id, UpdateMarketCategoryDto updateMarketCategoryDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}