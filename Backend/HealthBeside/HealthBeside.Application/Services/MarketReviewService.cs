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
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class MarketReviewService : IMarketReviewService
{
    private readonly IMarketReviewRepository _marketReviewRepository;
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly ILogger<MarketReviewService> _logger;

    public MarketReviewService(IMarketReviewRepository  marketReviewRepository,
        IMarketProductRepository marketProductRepository,
        IApplicationUserRepository userRepository,
        ILogger<MarketReviewService> logger)
    {
        _marketReviewRepository = marketReviewRepository;
        _marketProductRepository = marketProductRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<GetMarketReviewDto>> GetAllAsync(
        MarketReviewFilter? marketReviewFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        var queryable = _marketReviewRepository
            .GetQueryable();

        if (marketReviewFilter != null)
            queryable = queryable.Filter(marketReviewFilter);

        if (sortParams != null)
            queryable = queryable.Sort(sortParams);

        if (pageParams != null)
            queryable = queryable.Page(pageParams);

        var result = await queryable
            .AsNoTracking()
            .Include(r => r.User)
            .ToListAsync(cancellationToken);

        if (!result.Any())
            _logger.LogInformation("No market reviews found with current filters.");

        return result.Select(r => r.ToGetMarketReviewDto());
    }


    public async Task<GetMarketReviewDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var marketReview = await _marketReviewRepository.GetByIdWithAuthorAsync(id, cancellationToken);

        if(marketReview == null)
            throw new MarketReviewException("Market review not found");
        
        return marketReview.ToGetMarketReviewDto();
    }
    
    // TODO: ДОдати перевірку чи купляв юзер цей товар та чи не залишав вже відгук
    public async Task<GetMarketReviewDto> CreateAsync(CreateMarketReviewDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(dto.UserId, cancellationToken);
        
        if (user is null)
            throw new MarketReviewException($"User with id {dto.UserId} not found");

        var product = await _marketProductRepository.GetAsync(dto.ProductId, cancellationToken);
        
        if (product is null)
            throw new MarketReviewException($"Product with id {dto.ProductId} not found");
        
        (string? error, MarketReview? marketReview) =
            MarketReview.Create(dto.Description, dto.Rating, dto.ProductId, dto.UserId);
        
        if (error != null)
            throw new MarketReviewException(error);
        
        if(marketReview is null)
            throw new MarketReviewException("Unknown error market review is null");
        
        var addedReview = await _marketReviewRepository.AddAsync(marketReview, cancellationToken);
        var reviewWithAuthor = await _marketReviewRepository.GetByIdWithAuthorAsync(addedReview.Id, cancellationToken);
        
        if (reviewWithAuthor is null)
            throw new MarketReviewException($"Review with id {addedReview.Id} not found");
        
        return reviewWithAuthor.ToGetMarketReviewDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMarketReviewDto dto, CancellationToken cancellationToken = default)
    {
        var review = await _marketReviewRepository.GetAsync(id, cancellationToken);
        
        if(review is null)
            throw new MarketReviewException($"Market review with id {id} not found");

        var error = review.Update(dto.Description, dto.Rating);
        
        if (error != null)
            throw new MarketReviewException(error);
        
        await _marketReviewRepository.UpdateAsync(review, cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _marketReviewRepository.GetAsync(id, cancellationToken);

        if (review is null)
            return false;

        await _marketReviewRepository.DeleteAsync(id, cancellationToken);
        return true;
    }
}