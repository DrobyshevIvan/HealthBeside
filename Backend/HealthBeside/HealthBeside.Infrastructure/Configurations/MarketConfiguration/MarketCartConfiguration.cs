using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketCartConfiguration : IEntityTypeConfiguration<MarketCart>
{
    public void Configure(EntityTypeBuilder<MarketCart> builder)
    {
        builder.HasOne(c => c.User)
            .WithOne(u => u.MarketCart)
            .HasForeignKey<MarketCart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Deletes the cart if user was deleted
    }
}