using HealthBeside.Domain.Models.Marketplace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.MarketConfiguration;

public class MarketCategoryConfiguration : IEntityTypeConfiguration<MarketCategory>
{
    public void Configure(EntityTypeBuilder<MarketCategory> builder)
    {
        
    }    
}