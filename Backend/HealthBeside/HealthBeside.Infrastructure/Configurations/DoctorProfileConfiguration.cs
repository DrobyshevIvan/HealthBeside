using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations;

public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.HasOne(p => p.ApplicationUser)
            .WithOne(a => a.DoctorProfile)
            .HasForeignKey<DoctorProfile>(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}