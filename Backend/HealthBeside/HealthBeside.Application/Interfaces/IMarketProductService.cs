using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;

namespace HealthBeside.Application.Interfaces;

public interface IMarketProductService
{
    Task<PagedResult> GetAllAsync(
        MarketProductFilter? marketProductFilter, SortParams? sortParams, PageParams? pageParams, CancellationToken cancellationToken = default);
    Task<GetDetailedMarketProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetDetailedMarketProductDto> CreateAsync(CreateMarketProductDto createDto, CancellationToken cancellationToken = default);
    Task<GetDetailedMarketProductDto> UpdateAsync(Guid id, UpdateMarketProductDto updateMarketProductDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}