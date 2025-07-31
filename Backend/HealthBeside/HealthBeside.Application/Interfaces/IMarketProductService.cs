using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;

namespace HealthBeside.Application.Interfaces;

public interface IMarketProductService
{
    Task<IEnumerable<GetMarketProductDto>> GetAllAsync(MarketProductFilter? marketProductFilter, SortParams? sortParams, PageParams? pageParams);
    Task<GetDetailedMarketProductDto> GetByIdAsync(Guid id);
    Task<GetDetailedMarketProductDto> CreateAsync(CreateMarketProductDto createDto);
    Task<bool> UpdateAsync(Guid id, UpdateMarketProductDto updateMarketProductDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}