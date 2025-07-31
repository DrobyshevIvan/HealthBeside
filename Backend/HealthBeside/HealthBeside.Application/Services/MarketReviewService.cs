using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketProductDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketReviewDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Application.Services;

public class MarketReviewService : IMarketReviewService
{
    private readonly IMarketReviewRepository _marketReviewRepository;
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly IApplicationUserRepository _userRepository;

    public MarketReviewService(IMarketReviewRepository  marketReviewRepository,
        IMarketProductRepository marketProductRepository,
        IApplicationUserRepository userRepository)
    {
        _marketReviewRepository = marketReviewRepository;
        _marketProductRepository = marketProductRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<GetMarketReviewDto>> GetAllAsync(MarketReviewFilter? marketReviewFilter,
        SortParams? sortParams,
        PageParams? pageParams)
    {
        var queryable = _marketReviewRepository.GetQueryable();
        
        if(marketReviewFilter != null)
            queryable = queryable.Filter(marketReviewFilter);
        
        if(sortParams != null)
            queryable = queryable.Sort(sortParams);
        
        if(pageParams != null)
            queryable = queryable.Page(pageParams);
        
        var marketReviews = await queryable.ToListAsync(); 
        
        return marketReviews.Select(r => r.ToGetMarketReviewDto());
    }

    public async Task<GetMarketReviewDto> GetByIdAsync(Guid id)
    {
        var marketReview = await _marketReviewRepository.GetByIdWithAuthorAsync(id);

        if(marketReview == null)
            throw new MarketReviewException("Market review not found");
        
        return marketReview.ToGetMarketReviewDto();
    }

    public async Task<GetMarketReviewDto> CreateAsync(CreateMarketReviewDto dto)
    {
        var user = await _userRepository.GetAsync(dto.UserId);
        
        if (user is null)
            throw new MarketReviewException($"User with id {dto.UserId} not found");

        var product = await _marketProductRepository.GetAsync(dto.ProductId);
        
        if (product is null)
            throw new MarketReviewException($"Product with id {dto.ProductId} not found");
        
        (string? error, MarketReview? marketReview) =
            MarketReview.Create(dto.Description, dto.Rating, dto.ProductId, dto.UserId);
        
        if (error != null)
            throw new MarketReviewException(error);
        
        if(marketReview is null)
            throw new MarketReviewException("Unknown error market review is null");
        
        var addedReview = await _marketReviewRepository.AddAsync(marketReview);
        var reviewWithAuthor = await _marketReviewRepository.GetByIdWithAuthorAsync(addedReview.Id);
        
        if (reviewWithAuthor is null)
            throw new MarketReviewException($"Review with id {addedReview.Id} not found");
        
        return reviewWithAuthor.ToGetMarketReviewDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMarketReviewDto dto)
    {
        var review = await _marketReviewRepository.GetAsync(id);
        
        if(review is null)
            throw new MarketReviewException($"Market review with id {id} not found");

        var error = review.Update(dto.Description, dto.Rating);
        
        if (error != null)
            throw new MarketReviewException(error);
        
        await _marketReviewRepository.UpdateAsync(review);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var review = await _marketReviewRepository.GetAsync(id);

        if (review is null)
            return false;

        await _marketReviewRepository.DeleteAsync(id);
        return true;
    }
}