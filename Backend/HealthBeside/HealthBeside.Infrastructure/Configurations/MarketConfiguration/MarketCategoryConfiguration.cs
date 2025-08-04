using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketCategoryConfiguration : IEntityTypeConfiguration<MarketCategory>
{
    public void Configure(EntityTypeBuilder<MarketCategory> builder)
    {
        builder.Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .IsRequired();
    }    
}