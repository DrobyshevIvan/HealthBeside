using HealthBeside.Domain.Models.Forum;
using HealthBeside.Domain.Models.Shared;
using HealthBeside.Domain.Models.Users;
using HealthBeside.Infrastructure;
using HealthBeside.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace HealthBeside.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HealthBeside.API API",
                Version = "v1"
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("HealthBesideDbConnectionString")
            )
        );

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IForumCommentRepository, ForumCommentRepository>();
        builder.Services.AddScoped<IForumPostRepository, ForumPostRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthBeside.API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}