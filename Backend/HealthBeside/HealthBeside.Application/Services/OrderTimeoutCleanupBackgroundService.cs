using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Enums;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class OrderTimeoutCleanupBackgroundService : BackgroundService
{
    private readonly ILogger<OrderTimeoutCleanupBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(3);

    public OrderTimeoutCleanupBackgroundService(ILogger<OrderTimeoutCleanupBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupExpiredOrders(stoppingToken);

            var now = DateTime.UtcNow;
            
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IMarketOrderRepository>();
            
            var nearest = await orderRepository
                .GetNearestOrderOlderThan(now.AddMinutes(-15), OrderStatus.Pending, stoppingToken);

            if (nearest is null)
            {
                try { await Task.Delay(_checkInterval, stoppingToken); }
                catch (OperationCanceledException) { }
                continue;
            }

            var expiresAt = nearest.OrderDate.AddMinutes(15);
            now = DateTime.UtcNow;
            var delay = expiresAt - now;

            if (delay <= TimeSpan.Zero)
            {
                continue;
            }

            try { await Task.Delay(delay, stoppingToken); }
            catch (OperationCanceledException) { }
        }
    }
    
    private async Task CleanupExpiredOrders(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IMarketOrderService>();
        
        await orderService.CleanupExpiredOrders(cancellationToken);
        _logger.LogInformation("Expired orders cleaned up");
    }
}