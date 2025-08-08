using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.Cart;
using HealthBeside.Application.Extensions.Mapping.Marketplace.MarketCartDto.CartItem;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Users;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class MarketCartService : IMarketCartService
{
    private readonly IMarketCartRepository _marketCartRepository;
    private readonly IMarketCartItemRepository _marketCartItemRepository;
    private readonly ILogger<MarketCartService> _logger;

    public MarketCartService(
        IMarketCartRepository marketCartRepository,
        IMarketCartItemRepository marketCartItemRepository,
        ILogger<MarketCartService> logger)
    {
        _marketCartRepository = marketCartRepository;
        _marketCartItemRepository = marketCartItemRepository;
        _logger = logger;
    }

    private static void EnsureAccess(MarketCart cart, Guid userId, bool isAdmin)
    {
        if (!isAdmin && cart.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this cart.");
    }
    
    public async Task<GetDetailedCartDto> GetDetailedCart(Guid id, Guid currentUserId, bool isAdmin)
    {
        _logger.LogInformation("Fetching detailed cart with ID {CartId}", id);

        if (id == Guid.Empty)
            throw new MarketCartException("Cart ID cannot be empty.");

        var cart = await _marketCartRepository.GetByIdWithAllItems(id);
        if (cart == null)
        {
            _logger.LogWarning("Cart with ID {CartId} was not found", id);
            throw new MarketCartException($"Cart with id {id} was not found");
        }

        EnsureAccess(cart, currentUserId, isAdmin);


        _logger.LogInformation("Cart {CartId} retrieved successfully", id);
        return cart.ToGetDetailedCartDto();
    }


    public async Task<GetCartItemDto> AddProductToCart(Guid cartId, Guid productId, int quantity)
    {
        _logger.LogInformation("Adding product {ProductId} (quantity {Quantity}) to cart {CartId}", productId, quantity, cartId);

        if (cartId == Guid.Empty || productId == Guid.Empty)
            throw new MarketCartException("Invalid cart or product ID.");

        if (quantity <= 0)
            throw new MarketCartException("Quantity must be greater than zero.");

        var cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
        if (cart is null)
            throw new MarketCartException($"Cart with id {cartId} was not found");

        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

        if (cartItem is not null)
        {
            _logger.LogInformation("Product already exists in cart {CartId}, increasing quantity", cartId);
            var error = cartItem.AddQuantityToExistsItem(quantity);
            if (error != null)
                throw new MarketCartException(error);

            await _marketCartItemRepository.UpdateAsync(cartItem);
        }
        else
        {
            _logger.LogInformation("Adding new product {ProductId} to cart {CartId}", productId, cartId);
            var (errors, marketCartItem) = MarketCartItem.Create(quantity, productId, cartId);

            if (errors != null)
                throw new MarketCartException(errors);
            if (marketCartItem is null)
                throw new MarketCartException("Unknown error: MarketCartItem is null");

            await _marketCartItemRepository.AddAsync(marketCartItem);
            cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
            cartItem = cart?.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (cartItem is null)
                throw new MarketCartException("Unknown error: MarketCartItem not found after insert");
        }

        _logger.LogInformation("Product {ProductId} successfully added/updated in cart {CartId}", productId, cartId);
        return cartItem.ToGetCartItem();
    }

    public async Task<GetDetailedCartDto> UpdateProductInCart(Guid cartId, Guid productId, int quantity)
    {
        _logger.LogInformation("Updating product {ProductId} in cart {CartId} to quantity {Quantity}", productId, cartId, quantity);

        if (quantity <= 0)
            throw new MarketCartException("Quantity must be greater than zero.");

        var cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
        if (cart is null)
            throw new MarketCartException($"Cart with id {cartId} was not found");

        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem is null)
            throw new MarketCartException($"Product with id {productId} not found in cart {cartId}");

        var error = cartItem.Update(quantity);
        if (error != null)
            throw new MarketCartException(error);

        await _marketCartItemRepository.UpdateAsync(cartItem);
        _logger.LogInformation("Product {ProductId} in cart {CartId} updated successfully", productId, cartId);

        return cart.ToGetDetailedCartDto();
    }

    public async Task<bool> RemoveProductFromCart(Guid cartId, Guid productId)
    {
        _logger.LogInformation("Removing product {ProductId} from cart {CartId}", productId, cartId);

        var cart = await _marketCartRepository.GetByIdWithAllItems(cartId);
        if (cart is null)
            throw new MarketCartException($"Cart with id {cartId} was not found");

        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem is null)
        {
            _logger.LogWarning("Product {ProductId} not found in cart {CartId}", productId, cartId);
            return false;
        }

        cart.CartItems.Remove(cartItem);
        await _marketCartRepository.UpdateAsync(cart);

        _logger.LogInformation("Product {ProductId} removed from cart {CartId}", productId, cartId);
        return true;
    }

    public async Task<MarketCart> GetOrCreateCart(Guid userId)
    {
        _logger.LogInformation("Fetching or creating cart for user {UserId}", userId);

        var cart = await _marketCartRepository.GetByUserId(userId);
        if (cart is null)
        {
            _logger.LogInformation("No cart found for user {UserId}, creating new one", userId);

            var (error, marketCart) = MarketCart.Create(userId);
            if (error != null)
                throw new MarketCartException(error);
            if (marketCart is null)
                throw new MarketCartException("Unknown error: MarketCart is null");

            await _marketCartRepository.AddAsync(marketCart);
            cart = await _marketCartRepository.GetByUserId(userId);

            _logger.LogInformation("Cart created for user {UserId}", userId);
        }
        else
        {
            _logger.LogInformation("Existing cart found for user {UserId}", userId);
        }

        return cart;
    }
}
