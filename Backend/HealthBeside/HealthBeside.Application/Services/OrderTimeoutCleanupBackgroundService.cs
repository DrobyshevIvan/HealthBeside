using HealthBeside.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthBeside.Application.Services;

public class OrderTimeoutCleanupBackgroundService : BackgroundService
{
    private readonly ILogger<OrderTimeoutCleanupBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

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
            try
            {
                await CleanupExpiredOrders(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while cleaning up expired orders");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
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