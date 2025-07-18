using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations.UserConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid> { Id = Guid.Parse("aeec2a74-61cd-41eb-8ac7-251c365da8c5"), Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<Guid> { Id = Guid.Parse("098f240f-01cf-4925-ba10-4983724f73c3"), Name = "User", NormalizedName = "USER" },
            new IdentityRole<Guid> { Id = Guid.Parse("665c40a7-46ec-4564-a679-755f77c90472"), Name = "Doctor", NormalizedName = "DOCTOR" },
            new IdentityRole<Guid> { Id = Guid.Parse("c1167d0e-ff05-4df7-bfb5-69baf52c174f"), Name = "Patient", NormalizedName = "PATIENT" }
        );
    }
}