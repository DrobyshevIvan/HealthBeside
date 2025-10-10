using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Services;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Shared;
using HealthBeside.Infrastructure.Processors;
using HealthBeside.Infrastructure.Repositories;
using HealthBeside.Infrastructure.UnitOfWork;
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
        services.AddScoped<IMarketReviewRepository, MarketReviewRepository>();
        services.AddScoped<IMarketCartItemRepository, MarketCartItemRepository>();
        services.AddScoped<IMarketCartRepository, MarketCartRepository>();
        services.AddScoped<IMarketOrderRepository, MarketOrderRepository>();
        services.AddScoped<IMarketOrderItemRepository, MarketOrderItemRepository>();
        services.AddScoped<IUserDeliveryInfoRepository, UserDeliveryInfoRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IDoctorProfileRepository, DoctorProfileRepository>();
        services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IForumPostService, ForumPostService>();
        services.AddScoped<IForumCommentService, ForumCommentService>();
        services.AddScoped<IMarketProductService, MarketProductService>();
        services.AddScoped<IMarketCategoryService, MarketCategoryService>();
        services.AddScoped<IMarketReviewService, MarketReviewService>();
        services.AddScoped<IMarketCartService, MarketCartService>();
        services.AddScoped<IMarketOrderService, MarketOrderService>();
        services.AddScoped<IUserDeliveryInfoService, UserDeliveryInfoService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<IProfileService, ProfileService>();

        return services;
    }
}