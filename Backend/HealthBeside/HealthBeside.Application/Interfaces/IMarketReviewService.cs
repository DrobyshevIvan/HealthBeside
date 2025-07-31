using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IMarketReviewService
{
    Task<IEnumerable<GetMarketReviewDto>> GetAllAsync(MarketReviewFilter? marketReviewFilter, SortParams? sortParams, PageParams? pageParams);
    Task<GetMarketReviewDto> GetByIdAsync(Guid id);
    Task<GetMarketReviewDto> CreateAsync(CreateMarketReviewDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateMarketReviewDto dto);
    Task<bool> DeleteAsync(Guid id);
}