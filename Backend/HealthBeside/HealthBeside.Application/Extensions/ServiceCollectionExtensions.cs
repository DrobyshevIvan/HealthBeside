using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Services;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Shared;
using HealthBeside.Infrastructure.Processors;
using HealthBeside.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HealthBeside.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        // Processors
        services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IForumCommentRepository, ForumCommentRepository>();
        services.AddScoped<IForumPostRepository, ForumPostRepository>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IMarketProductRepository, MarketProductRepository>();
        services.AddScoped<IMarketCategoryRepository, MarketCategoryRepository>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IForumPostService, ForumPostService>();
        services.AddScoped<IForumCommentService, ForumCommentService>();
        services.AddScoped<IMarketProductService, MarketProductService>();
        services.AddScoped<IMarketCategoryService, MarketCategoryService>();

        return services;
    }
}