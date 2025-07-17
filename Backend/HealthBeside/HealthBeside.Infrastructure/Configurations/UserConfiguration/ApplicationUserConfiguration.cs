using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.UserConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.RegistrationDate)
            .HasDefaultValueSql("NOW()");
        
        builder.Property(u => u.FirstName)
            .HasMaxLength(55);
        
        builder.Property(u => u.LastName)
            .HasMaxLength(55);
    }
}