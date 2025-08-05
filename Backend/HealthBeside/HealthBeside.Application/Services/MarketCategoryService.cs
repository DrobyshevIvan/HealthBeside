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
        var categories = await _marketCategoryRepository.GetAllAsync(cancellationToken);
        
        /*if(categories.Count == 0)
        {
            _logger.LogInformation("No market categories found.");
            return Enumerable.Empty<GetMarketCategoryDto>();
        }*/
            
        return categories.Select(c => c.ToGetCategoryDto());
    }

    public async Task<GetMarketCategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);

        if (category is null)
            throw new MarketCategoryException($"Category with id {id} was not found");
        
        return category.ToGetCategoryDto();
    }

    public async Task<GetMarketCategoryDto> CreateAsync(
        CreateMarketCategoryDto createMarketCategoryDto, 
        CancellationToken cancellationToken = default)
    {
        (string? error, MarketCategory? marketCategory) = MarketCategory.Create(
            createMarketCategoryDto.Name, createMarketCategoryDto.Description);
        
        if (error != null)
            throw new MarketCategoryException(error);
        
        if (marketCategory is null)
            throw new MarketCategoryException("Unknown error market category is null");
        
        var addedCategory = await _marketCategoryRepository.AddAsync(marketCategory, cancellationToken);
        
        return addedCategory.ToGetCategoryDto();
    }

    public async Task<bool> UpdateAsync(
        Guid id, 
        UpdateMarketCategoryDto updateMarketCategoryDto,
        CancellationToken cancellationToken = default)
    {
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);
        
        if (category is null)
            throw new MarketCategoryException($"Category with id {id} was not found");
        
        var error = category.Update(updateMarketCategoryDto.Name, updateMarketCategoryDto.Description);
        
        if (error != null)
            throw new MarketCategoryException(error);

        await _marketCategoryRepository.UpdateAsync(category, cancellationToken);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _marketCategoryRepository.GetAsync(id, cancellationToken);

        if (category is null)
            return false;

        try
        {
            await _marketCategoryRepository.DeleteAsync(id, cancellationToken);
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
        var product = await _marketCategoryRepository.GetAsync(id, cancellationToken);
        
        if (product is null)
            return false;
        
        return true;
    }
}