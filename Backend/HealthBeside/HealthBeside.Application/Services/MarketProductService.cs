using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
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

public class MarketProductService : IMarketProductService
{
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly ILogger<MarketProductService> _logger;

    public MarketProductService(IMarketProductRepository marketProductRepository, ILogger<MarketProductService> logger)
    {
        _marketProductRepository = marketProductRepository;
        _logger = logger;
    }

    public async Task<PagedResult> GetAllAsync(MarketProductFilter? marketProductFilter,
        SortParams? sortParams,
        PageParams? pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all market products with filters applied.");

        var query = _marketProductRepository.GetQueryable();

        if (marketProductFilter != null)
            query = query.Filter(marketProductFilter);

        if (sortParams != null)
            query = query.Sort(sortParams);

        var total = query.Count();
        
        if (pageParams != null)
            query = query.Page(pageParams);

        var products = await query.Select(p => new GetMarketProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            SKU = p.SKU,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            AverageRating = p.Reviews.Average(r => r.Rating),
            ReviewsCount = p.Reviews.Count()
        }).ToListAsync(cancellationToken);
        
        return new PagedResult
        {
            
            Products = products,
            Total = total
        };
    }

    public async Task<GetDetailedMarketProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving market product by ID: {ProductId}", id);

        var product = await _marketProductRepository.GetByIdWithCategoryAsync(id, cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Market product with ID {ProductId} not found.", id);
            throw new MarketProductCreationException($"Product with id {id} was not found");
        }

        _logger.LogInformation("Market product {ProductId} retrieved successfully.", id);

        return product.ToGetDetailedMarketProductDto();
    }

    public async Task<GetDetailedMarketProductDto> CreateAsync(CreateMarketProductDto createDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating market product: {Name}", createDto.Name);

        (string? error, MarketProduct? marketProduct) = MarketProduct.Create(
            createDto.Name,
            createDto.Description,
            createDto.Price,
            createDto.Quantity,
            createDto.SKU,
            createDto.ImageUrl,
            createDto.CategoryId);

        if (error != null)
        {
            _logger.LogWarning("Validation failed while creating market product: {Error}", error);
            throw new MarketProductCreationException(error);
        }

        if (marketProduct is null)
        {
            _logger.LogError("Market product creation failed: null instance returned.");
            throw new MarketProductCreationException("Unknown error market product is null");
        }

        var addedProduct = await _marketProductRepository.AddAsync(marketProduct, cancellationToken);

        var productWithCategory = await _marketProductRepository.GetByIdWithCategoryAsync(addedProduct.Id, cancellationToken);

        if (productWithCategory is null)
        {
            _logger.LogError("Market product created but failed to retrieve with category. ProductId: {ProductId}", addedProduct.Id);
            throw new MarketProductCreationException($"Product with id {addedProduct.Id} was not found");
        }

        _logger.LogInformation("Market product {ProductId} created successfully.", addedProduct.Id);

        return productWithCategory.ToGetDetailedMarketProductDto();
    }

    public async Task<GetDetailedMarketProductDto> UpdateAsync(Guid id, UpdateMarketProductDto updateMarketProductDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating market product {ProductId}", id);

        var product = await _marketProductRepository.GetByIdWithCategoryAsync(id, cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Attempted to update non-existent market product {ProductId}", id);
            throw new MarketProductCreationException($"Product with id {id} was not found");
        }

        var error = product.Update(
            updateMarketProductDto.Name,
            updateMarketProductDto.Description,
            updateMarketProductDto.Price,
            updateMarketProductDto.Quantity,
            updateMarketProductDto.SKU,
            updateMarketProductDto.ImageUrl,
            updateMarketProductDto.CategoryId);

        if (error != null)
        {
            _logger.LogWarning("Validation failed during market product update {ProductId}: {Error}", id, error);
            throw new MarketProductCreationException(error);
        }

        await _marketProductRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Market product {ProductId} updated successfully.", id);

        return product.ToGetDetailedMarketProductDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete market product {ProductId}", id);

        var product = await _marketProductRepository.GetAsync(id, cancellationToken);

        if (product is null)
        {
            _logger.LogWarning("Market product {ProductId} not found for deletion.", id);
            return false;
        }

        await _marketProductRepository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("Market product {ProductId} deleted successfully.", id);

        return true;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _marketProductRepository.GetAsync(id, cancellationToken) is not null;
        _logger.LogDebug("Existence check for market product {ProductId}: {Exists}", id, exists);
        return exists;
    }
}
