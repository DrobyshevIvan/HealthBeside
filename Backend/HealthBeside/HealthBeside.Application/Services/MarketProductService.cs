using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketProductDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Services;

public class MarketProductService : IMarketProductService
{
    private readonly IMarketProductRepository _marketProductRepository;

    public MarketProductService(IMarketProductRepository marketProductRepository)
    {
        _marketProductRepository = marketProductRepository;
    }

    public async Task<IEnumerable<GetMarketProductDto>> GetAllAsync()
    {
        var products = await _marketProductRepository.GetAllAsync();
        return products.Select(p => new GetMarketProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            SKU = p.SKU,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId
        });
    }

    public async Task<GetDetailedMarketProductDto> GetByIdAsync(Guid id)
    {
        var product = await _marketProductRepository.GetByIdWithCategoryAsync(id);
        
        if (product is null)
            throw new MarketProductCreationException($"Product with id {id} was not found");
        
        return product.ToGetDetailedMarketProductDto();
    }

    public async Task<GetDetailedMarketProductDto> CreateAsync(CreateMarketProductDto createDto)
    {
        (string? error, MarketProduct? marketProduct) = MarketProduct.Create(
            createDto.Name, createDto.Description,  createDto.Price, createDto.Quantity,  createDto.SKU, createDto.ImageUrl,  createDto.CategoryId);
        
        if (error != null) 
            throw new MarketProductCreationException(error);
        
        if (marketProduct is null) 
            throw new MarketProductCreationException("Unknown error market product is null");
        
        var addedProduct = await _marketProductRepository.AddAsync(marketProduct);
        
        var productWithCategory = await _marketProductRepository.GetByIdWithCategoryAsync(addedProduct.Id);
        
        if (productWithCategory is null)
            throw new MarketProductCreationException($"Product with id {addedProduct.Id} was not found");
        
        return productWithCategory.ToGetDetailedMarketProductDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMarketProductDto updateMarketProductDto)
    {
        var product = await _marketProductRepository.GetAsync(id);
        
        if (product is null)
            throw new MarketProductCreationException($"Product with id {id} was not found");
        
        var error = product.Update(updateMarketProductDto.Name, updateMarketProductDto.Description, updateMarketProductDto.Price, updateMarketProductDto.Quantity,
            updateMarketProductDto.SKU, updateMarketProductDto.ImageUrl, updateMarketProductDto.CategoryId);
        
        if (error != null)
            throw new MarketProductCreationException(error);
        
        await _marketProductRepository.UpdateAsync(product);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _marketProductRepository.GetAsync(id);
        
        if (product is null)
            return false;

        await _marketProductRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var product = await _marketProductRepository.GetAsync(id);
        
        if(product is null) 
            return false;

        return true;
    }
}