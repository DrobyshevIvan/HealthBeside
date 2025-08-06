using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IMarketReviewService
{
    Task<IEnumerable<GetMarketReviewDto>> GetAllAsync(
        MarketReviewFilter? marketReviewFilter, 
        SortParams? sortParams, 
        PageParams? pageParams,
        CancellationToken cancellationToken = default);
    Task<GetMarketReviewDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetMarketReviewDto> CreateAsync(CreateMarketReviewDto dto, Guid userId, CancellationToken cancellationToken = default);
    Task<GetMarketReviewDto> UpdateAsync(
        Guid reviewId,
        Guid currentUserId,
        UpdateMarketReviewDto dto,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid reviewId, Guid currentUserId, CancellationToken cancellationToken = default);
}