using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.Cart;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.CartItem;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Services;

public class MarketCartService : IMarketCartService
{
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketProductRepository _marketProductRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;

    public MarketCartService(IMarketCartRepository marketCartRepository,
        IMarketProductRepository marketProductRepository,
        IMarketCartItemRepository marketCartItemRepository)
    {
        _marketCartRepository = marketCartRepository;
        _marketProductRepository = marketProductRepository;
        _marketCartItemRepository = marketCartItemRepository;
    }
    
    public async Task<GetDetailedCartDto> GetDetailedCart(Guid id)
    {
        var cart = await _marketCartRepository.GetByIdWithAllItems(id);

        if (cart == null)
            throw new MarketCartException($"Exception in getting cart with id {id}");
        
        return cart.ToGetDetailedCartDto();
    }

    public async Task<GetCartItemDto> AddProductToCart(Guid cartId, Guid productId, int quantity)
    {
        var cart =  await _marketCartRepository.GetByIdWithAllItems(cartId);

        if (cart is null) 
            throw new MarketCartException($"Cart with id {cartId} was not found");

        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
        
        if (cartItem is not null)
        {
            var error = cartItem.AddQuantityToExistsItem(quantity);
            
            if (error != null)
                throw new MarketCartException(error);
            
            await _marketCartItemRepository.UpdateAsync(cartItem);
        }
        else
        {
            (string? errors, MarketCartItem? marketCartItem) = MarketCartItem.Create(quantity, productId, cartId);
            
            if (errors != null)
                throw new MarketCartException(errors);
            
            if (marketCartItem is null)
                throw new MarketCartException("Unknown error MarketCartItem is null");
            
            var added = await _marketCartItemRepository.AddAsync(marketCartItem);
            cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
            
            if (cart is null)
                throw new MarketCartException($"Unknown error cart with id {cartId} was not found");
            
            cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem is null)
                throw new MarketCartException("Unknown error MarketCartItem is null");
        }
        
        return cartItem.ToGetCartItem();
    }

    // TODO: Додати метод для оновлення поточного кошику
    public async Task<bool> UpdateProductInCart(Guid cartId, Guid productId, int quantity)
    {
        var cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
        
        if (cart is null) 
            throw new MarketCartException($"Cart with id {cartId} was not found");
        
        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

        if (cartItem is not null)
        {
            var error = cartItem.Update(quantity);
            
            if (error != null)
                throw new MarketCartException(error);
            
            await _marketCartItemRepository.UpdateAsync(cartItem);
            
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveProductFromCart(Guid cartId, Guid productId)
    {
        var cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
        
        if (cart is null)
            throw new MarketCartException($"Cart with id {cartId} was not found");
        
        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

        if (cartItem is not null)
        {
            cart.CartItems.Remove(cartItem);
            await _marketCartRepository.UpdateAsync(cart);
            return true;
        }

        return false;
    }

    public async Task<MarketCart> GetOrCreateCart(Guid userId)
    {
        var cart = await _marketCartRepository.GetByUserId(userId);

        if (cart is null)
        {
            (string? error, MarketCart? marketCart) = MarketCart.Create(userId);
        
            if (error != null)
                throw new MarketCartException(error);
        
            if (marketCart is null)
                throw new MarketCartException("Unknown error MarketCart is null");
        
            await _marketCartRepository.AddAsync(marketCart);
            
            cart = await _marketCartRepository.GetByUserId(userId);
        }

        return cart;
    }
}