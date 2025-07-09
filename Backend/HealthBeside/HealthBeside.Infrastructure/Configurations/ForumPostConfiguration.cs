using HealthBeside.Domain.Models.Forum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations;

public class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
{
    public void Configure(EntityTypeBuilder<ForumPost> builder)
    {
        builder.HasOne(p => p.Author)
            .WithMany(a => a.ForumPosts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of the author if they have related forum posts
        
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
        
        builder.Property(p => p.Likes)
            .HasDefaultValue(0);
        
        builder.Property(p => p.Dislikes)
            .HasDefaultValue(0);
        
        
    }
}