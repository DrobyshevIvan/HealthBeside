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

    public MarketReviewService(IMarketReviewRepository marketReviewRepository,
        IMarketProductRepository marketProductRepository,
        IApplicationUserRepository userRepository,
        ILogger<MarketReviewService> logger)
    {
        _marketReviewRepository = marketReviewRepository;
        _marketProductRepository = marketProductRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    private static void EnsureOwnership(MarketReview review, Guid userId)
    {
        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
    }
    
    public async Task<IEnumerable<GetMarketReviewDto>> GetAllAsync(
        MarketReviewFilter? marketReviewFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve all market reviews with filters: {Filter}, sort: {Sort}, page: {Page}", 
            marketReviewFilter?.ToString(), sortParams?.ToString(), pageParams?.ToString());

        var queryable = _marketReviewRepository
            .GetQueryable();

        if (marketReviewFilter != null)
        {
            queryable = queryable.Filter(marketReviewFilter);
            _logger.LogDebug("Applied filter: {Filter}", marketReviewFilter.ToString());
        }

        if (sortParams != null)
        {
            queryable = queryable.Sort(sortParams);
            _logger.LogDebug("Applied sort: {Sort}", sortParams.ToString());
        }

        if (pageParams != null)
        {
            queryable = queryable.Page(pageParams);
            _logger.LogDebug("Applied pagination: {Page}", pageParams.ToString());
        }

        var result = await queryable
            .AsNoTracking()
            .Include(r => r.User)
            .ToListAsync(cancellationToken);

        if (!result.Any())
            _logger.LogInformation("No market reviews found with current criteria.");
        else
            _logger.LogInformation("Successfully retrieved {Count} market reviews.", result.Count());

        return result.Select(r => r.ToGetMarketReviewDto());
    }


    public async Task<GetMarketReviewDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve market review with ID: {ReviewId}.", id);
        
        var marketReview = await _marketReviewRepository.GetByIdWithAuthorAsync(id, cancellationToken);

        if(marketReview == null)
        {
            _logger.LogWarning("Market review with ID {ReviewId} not found.", id);
            throw new MarketReviewException($"Market review with ID {id} not found.");
        }
        
        _logger.LogInformation("Successfully retrieved market review with ID: {ReviewId}.", id);
        return marketReview.ToGetMarketReviewDto();
    }
    
    public async Task<GetMarketReviewDto> CreateAsync(CreateMarketReviewDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create a new market review for ProductId: {ProductId} by UserId: {UserId}.", 
            dto.ProductId, userId);

        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found when creating review.", userId);
            throw new MarketReviewException($"User with id {userId} not found");
        }

        var product = await _marketProductRepository.GetAsync(dto.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found when creating review.", dto.ProductId);
            throw new MarketReviewException($"Product with id {dto.ProductId} not found");
        }
        
        var existingReview = await _marketReviewRepository
            .GetQueryable()
            .AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId, cancellationToken);

        if (existingReview)
        {
            _logger.LogWarning("User {UserId} already submitted a review for ProductId {ProductId}.", userId, dto.ProductId);
            throw new MarketReviewException("You have already submitted a review for this product."); //TODO зробити так, щоб воно показувалось на стороні юзера, а не лише в консолі
        }
        
        (string? error, MarketReview? marketReview) =
            MarketReview.Create(dto.Description, dto.Rating, dto.ProductId, userId);
        
        if (error != null)
        {
            _logger.LogWarning("Validation failed for new market review: {Error}", error);
            throw new MarketReviewException(error);
        }
        
        if(marketReview is null)
        {
            _logger.LogError("MarketReview.Create returned null without an error message. Unexpected state.");
            throw new MarketReviewException("Unknown error: market review is null");
        }
        
        var addedReview = await _marketReviewRepository.AddAsync(marketReview, cancellationToken);
        
        var reviewWithAuthor = await _marketReviewRepository.GetByIdWithAuthorAsync(addedReview.Id, cancellationToken);
        
        if (reviewWithAuthor is null)
        {
            _logger.LogError("Newly added review with ID {ReviewId} not found after creation. This indicates a data consistency issue.", addedReview.Id);
            throw new MarketReviewException($"Review with id {addedReview.Id} not found after creation.");
        }
        
        _logger.LogInformation("Successfully created new market review with ID: {ReviewId} for ProductId: {ProductId}.", 
            addedReview.Id, addedReview.ProductId);
        
        return reviewWithAuthor.ToGetMarketReviewDto();
    }

    public async Task<GetMarketReviewDto> UpdateAsync(
        Guid reviewId,
        Guid currentUserId,
        UpdateMarketReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update review {ReviewId} by user {UserId}", reviewId, currentUserId);

        var review = await _marketReviewRepository
            .GetQueryable()
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);


        if (review is null)
        {
            _logger.LogWarning("Review with id {ReviewId} not found for update.", reviewId);
            throw new MarketReviewException($"Market review with id {reviewId} not found.");
        }

        EnsureOwnership(review, currentUserId); // Логування тут вже є у методі EnsureOwnership, якщо він не статичний або має доступ до логера

        var error = review.Update(dto.Description, dto.Rating);

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogWarning("Validation failed for review {ReviewId}: {Error}", reviewId, error);
            throw new MarketReviewException(error);
        }

        await _marketReviewRepository.UpdateAsync(review, cancellationToken);

        _logger.LogInformation("Review {ReviewId} successfully updated by user {UserId}", reviewId, currentUserId);
        return review.ToGetMarketReviewDto();
    }


    public async Task<bool> DeleteAsync(Guid reviewId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete market review with ID: {ReviewId}.", reviewId);
        
        var review = await _marketReviewRepository.GetAsync(reviewId, cancellationToken);

        if (review is null)
        {
            _logger.LogWarning("Market review with ID {ReviewId} not found for deletion.", reviewId);
            return false;
        }

        EnsureOwnership(review, currentUserId);
        
        await _marketReviewRepository.DeleteAsync(reviewId, cancellationToken);
        _logger.LogInformation("Successfully deleted market review with ID: {ReviewId}.", reviewId);
        return true;
    }
}