using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCategoryDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class MarketCategoryService : IMarketCategoryService
{
    private readonly IMarketCategoryRepository _marketCategoryRepository;
    private readonly ILogger<MarketCategoryService> _logger;

    public MarketCategoryService(IMarketCategoryRepository marketCategoryRepository, ILogger<MarketCategoryService> logger)
    {
        _marketCategoryRepository = marketCategoryRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<GetMarketCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve all market categories.");
        var categories = await _marketCategoryRepository.GetAllAsync(cancellationToken);
        
        if (!categories.Any())
        {
            _logger.LogInformation("No market categories found.");
            return Enumerable.Empty<GetMarketCategoryDto>();
        }
        
        _logger.LogInformation("Successfully retrieved {Count} market categories.", categories.Count());
        return categories.Select(c => c.ToGetCategoryDto());
    }

    public async Task<GetMarketCategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to retrieve market category with id {Id}.", id);
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);

        if (category is null)
        {
            _logger.LogWarning("Market category with id {Id} not found.", id);
            throw new MarketCategoryException($"Category with id {id} was not found");
        }
        
        _logger.LogInformation("Successfully retrieved market category with id {Id}.", id);
        return category.ToGetCategoryDto();
    }

    public async Task<GetMarketCategoryDto> CreateAsync(
        CreateMarketCategoryDto createMarketCategoryDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create a new market category with name {Name}.", createMarketCategoryDto.Name);
        
        var existingCategory = (await _marketCategoryRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(c => c.Name.Equals(createMarketCategoryDto.Name, StringComparison.OrdinalIgnoreCase));

        if (existingCategory != null)
        {
            _logger.LogWarning("Failed to create market category. A category with name '{Name}' already exists.", createMarketCategoryDto.Name);
            throw new MarketCategoryException($"A category with name '{createMarketCategoryDto.Name}' already exists.");
        }
        
        (string? error, MarketCategory? marketCategory) = MarketCategory.Create(
            createMarketCategoryDto.Name, createMarketCategoryDto.Description);
        
        if (error != null)
        {
            _logger.LogError("Failed to create market category due to validation error: {Error}", error);
            throw new MarketCategoryException(error);
        }
        
        if (marketCategory is null)
        {
            _logger.LogError("Unknown error: Market category object is null after creation attempt.");
            throw new MarketCategoryException("Unknown error market category is null");
        }
        
        var addedCategory = await _marketCategoryRepository.AddAsync(marketCategory, cancellationToken);
        
        _logger.LogInformation("Successfully created market category with id {Id}.", addedCategory.Id);
        return addedCategory.ToGetCategoryDto();
    }

    public async Task<GetMarketCategoryDto> UpdateAsync(
        Guid id, 
        UpdateMarketCategoryDto updateMarketCategoryDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update market category with id {Id}.", id);
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);
        
        if (category is null)
        {
            _logger.LogWarning("Market category with id {Id} not found for update.", id);
            throw new MarketCategoryException($"Category with id {id} was not found");
        }
        
        var error = category.Update(updateMarketCategoryDto.Name, updateMarketCategoryDto.Description);
        
        if (error != null)
        {
            _logger.LogError("Failed to update market category {Id} due to validation error: {Error}", id, error);
            throw new MarketCategoryException(error);
        }

        await _marketCategoryRepository.UpdateAsync(category, cancellationToken);
        
        _logger.LogInformation("Successfully updated market category with id {Id}.", id);
        return category.ToGetCategoryDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete market category with id {Id}.", id);
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);

        if (category is null)
        {
            _logger.LogWarning("Delete failed: Market category with id {Id} not found.", id);
            return false;
        }

        try
        {
            await _marketCategoryRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Successfully deleted market category with id {Id}.", id);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to delete category {CategoryId} due to related products.", id);
        
            throw new InvalidOperationException($"Cannot delete category with ID {id} because it has associated products.", ex);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if market category with id {Id} exists.", id);
        var product = await _marketCategoryRepository.GetAsync(id, cancellationToken);
        
        if (product is null)
        {
            _logger.LogInformation("Market category with id {Id} does not exist.", id);
            return false;
        }
        
        _logger.LogInformation("Market category with id {Id} exists.", id);
        return true;
    }
}
