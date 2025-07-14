using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketOrderConfiguration : IEntityTypeConfiguration<MarketOrder>
{
    public void Configure(EntityTypeBuilder<MarketOrder> builder)
    {
        builder.HasOne(o => o.User)
            .WithMany(u => u.MarketOrders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}