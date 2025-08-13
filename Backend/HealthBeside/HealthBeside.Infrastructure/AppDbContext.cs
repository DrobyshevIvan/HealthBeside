using HealthBeside.Domain.Models.Forum;
using HealthBeside.Domain.Models.Marketplace;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure.Configurations.ForumConfiguration;
using HealthBeside.Infrastructure.Configurations.MarketConfiguration;
using HealthBeside.Infrastructure.Configurations.UserConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HealthBeside.Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<DoctorProfile> DoctorProfiles { get; set; }
    public DbSet<PatientProfile> PatientProfiles { get; set; }
    public DbSet<UserDeliveryInfo> UserDeliveryInfos { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumComment> ForumComments { get; set; }
    public DbSet<MarketCart> MarketCarts { get; set; }
    public DbSet<MarketCartItem> MarketCartItems { get; set; }
    public DbSet<MarketCategory> MarketCategories { get; set; }
    public DbSet<MarketOrder> MarketOrders { get; set; }
    public DbSet<MarketOrderItem> MarketOrderItems { get; set; }
    public DbSet<MarketProduct> MarketProducts { get; set; }
    public DbSet<MarketReview> MarketReviews { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorProfileConfiguration());
        modelBuilder.ApplyConfiguration(new ForumCommentConfiguration());
        modelBuilder.ApplyConfiguration(new ForumPostConfiguration());
        modelBuilder.ApplyConfiguration(new PatientProfileConfiguration());
        modelBuilder.ApplyConfiguration(new MarketCartConfiguration());
        modelBuilder.ApplyConfiguration(new MarketCartItemConfiguration());
        modelBuilder.ApplyConfiguration(new MarketProductConfiguration());
        modelBuilder.ApplyConfiguration(new MarketReviewConfiguration());
        modelBuilder.ApplyConfiguration(new MarketOrderConfiguration());
        modelBuilder.ApplyConfiguration(new MarketOrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new MarketProductConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserDeliveryInfoConfiguration());
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var conn = config.GetConnectionString("HealthBesideDbConnectionString");
        optionsBuilder.UseNpgsql(conn);
        return new AppDbContext(optionsBuilder.Options);
    }
}
