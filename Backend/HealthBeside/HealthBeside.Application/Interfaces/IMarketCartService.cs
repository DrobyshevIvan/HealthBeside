using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Domain.Models.Marketplace;

namespace HealthBeside.Application.Interfaces;

public interface IMarketCartService
{
    Task<GetDetailedCartDto> GetDetailedCart(Guid id, Guid currentUserId, bool isAdmin);
    Task<GetCartItemDto> AddProductToCart(Guid cartId, Guid productId, int quantity);
    Task<bool> RemoveProductFromCart(Guid cartId, Guid productId);
    Task<MarketCart> GetOrCreateCart(Guid userId);
    Task<GetDetailedCartDto> UpdateProductInCart(Guid cartId, Guid productId, int quantity);
}