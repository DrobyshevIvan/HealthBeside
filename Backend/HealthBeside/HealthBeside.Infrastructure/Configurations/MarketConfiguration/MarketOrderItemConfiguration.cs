using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketOrderItemConfiguration : IEntityTypeConfiguration<MarketOrderItem>
{
    public void Configure(EntityTypeBuilder<MarketOrderItem> builder)
    {
        builder.HasOne(oi => oi.MarketProduct)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(oi => oi.MarketOrder)
            .WithMany(o => o.MarketOrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}