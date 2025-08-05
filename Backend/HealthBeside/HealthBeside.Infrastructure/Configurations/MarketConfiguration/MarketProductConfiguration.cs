using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketProductConfiguration : IEntityTypeConfiguration<MarketProduct>
{
    public void Configure(EntityTypeBuilder<MarketProduct> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(25);
        
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(255);
        
        builder.HasOne(o => o.Category)
            .WithMany(c => c.MarketProducts)
            .HasForeignKey(o => o.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(p => p.ImageUrl)
            .IsRequired(false);
    }
}