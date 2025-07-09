using HealthBeside.Domain.Models.Forum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthBeside.Infrastructure.Configurations;

public class ForumCommentConfiguration : IEntityTypeConfiguration<ForumComment>
{
    public void Configure(EntityTypeBuilder<ForumComment> builder)
    {
        builder.HasOne(c => c.Author)
            .WithMany(a => a.ForumComments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of the author if they have related forum comments
        
        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade); // Deletes comments when the post is deleted

        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deletion of the parent comment if it has replies
        
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Sets the default value for CreatedAt to the current date and time
        
        builder.Property(c => c.Likes).HasDefaultValue(0);
        
        builder.Property(c => c.Dislikes).HasDefaultValue(0);
        
        builder.Property(c => c.IsAnswer)
            .HasDefaultValue(false); 
            
    }
}