using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCategoryDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Services;

public class MarketCategoryService : IMarketCategoryService
{
    private readonly IMarketCategoryRepository _marketCategoryRepository;
    
    public MarketCategoryService(IMarketCategoryRepository marketCategoryRepository)
    {
        _marketCategoryRepository = marketCategoryRepository;
    }
    
    public async Task<IEnumerable<GetMarketCategoryDto>> GetAllAsync()
    {
        var categories = await _marketCategoryRepository.GetAllAsync();
        return categories.Select(c => c.ToGetCategoryDto());
    }

    public async Task<GetMarketCategoryDto> GetByIdAsync(Guid id)
    {
        var category = await _marketCategoryRepository.GetAsync(id);

        if (category is null)
            throw new MarketCategoryException($"Category with id {id} was not found");
        
        return category.ToGetCategoryDto();
    }

    public async Task<GetMarketCategoryDto> CreateAsync(CreateMarketCategoryDto createMarketCategoryDto)
    {
        (string? error, MarketCategory? marketCategory) = MarketCategory.Create(
            createMarketCategoryDto.Name, createMarketCategoryDto.Description);
        
        if (error != null)
            throw new MarketCategoryException(error);
        
        if (marketCategory is null)
            throw new MarketCategoryException("Unknown error market category is null");
        
        var addedCategory = await _marketCategoryRepository.AddAsync(marketCategory);
        
        return addedCategory.ToGetCategoryDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMarketCategoryDto updateMarketCategoryDto)
    {
        var category = await _marketCategoryRepository.GetAsync(id);
        
        if (category is null)
            throw new MarketCategoryException($"Category with id {id} was not found");
        
        var error = category.Update(updateMarketCategoryDto.Name, updateMarketCategoryDto.Description);
        
        if (error != null)
            throw new MarketCategoryException(error);

        await _marketCategoryRepository.UpdateAsync(category);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _marketCategoryRepository.GetAsync(id);

        if (category is null)
            return false;
        
        await _marketCategoryRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var product = await _marketCategoryRepository.GetAsync(id);
        
        if (product is null)
            return false;
        
        return true;
    }
}