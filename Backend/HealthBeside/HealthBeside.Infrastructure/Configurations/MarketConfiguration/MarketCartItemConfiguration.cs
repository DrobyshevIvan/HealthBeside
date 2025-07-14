using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketCartItemConfiguration : IEntityTypeConfiguration<MarketCartItem>
{
    public void Configure(EntityTypeBuilder<MarketCartItem> builder)
    {
        builder.HasOne(ci => ci.MarketProduct)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasOne(ci => ci.MarketCart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}