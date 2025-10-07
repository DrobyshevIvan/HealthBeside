using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.UserConfiguration;

public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
{
    public void Configure(EntityTypeBuilder<PatientProfile> builder)
    {
        builder.Property(p => p.MedicalHistorySummary)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.HasOne(p => p.ApplicationUser)
            .WithOne(a => a.PatientProfile)
            .HasForeignKey<PatientProfile>(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }    
}