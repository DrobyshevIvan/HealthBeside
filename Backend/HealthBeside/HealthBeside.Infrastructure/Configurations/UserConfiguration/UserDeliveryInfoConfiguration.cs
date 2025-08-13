using HealthBeside.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserDeliveryInfoConfiguration : IEntityTypeConfiguration<UserDeliveryInfo>
{
    public void Configure(EntityTypeBuilder<UserDeliveryInfo> builder)
    {
        // Встановлення первинного ключа
        builder.HasKey(udi => udi.Id);

        // Налаштування властивостей
        builder.Property(udi => udi.City).IsRequired().HasMaxLength(100);
        builder.Property(udi => udi.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(udi => udi.PostalIndex).IsRequired();
        builder.Property(udi => udi.StreetName).IsRequired().HasMaxLength(200);
        builder.Property(udi => udi.StreetNumber).IsRequired();

        builder.HasOne(udi => udi.ApplicationUser)
            .WithOne(u => u.UserDeliveryInfo)
            .HasForeignKey<UserDeliveryInfo>(udi => udi.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}