using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IMarketCategoryService
{
    Task<IEnumerable<GetMarketCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GetMarketCategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetMarketCategoryDto> CreateAsync(CreateMarketCategoryDto createMarketCategoryDto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateMarketCategoryDto updateMarketCategoryDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}