using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketReviewConfiguration : IEntityTypeConfiguration<MarketReview>
{
    public void Configure(EntityTypeBuilder<MarketReview> builder)
    {
        builder.HasOne(r => r.User)
            .WithMany(u => u.MarketReviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.MarketProduct)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}