namespace HealthBeside.API.Middlewares;

public class TaskCancellationHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TaskCancellationHandlingMiddleware> _logger;

    public TaskCancellationHandlingMiddleware(RequestDelegate next, ILogger<TaskCancellationHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when(ex is OperationCanceledException or TaskCanceledException)
        {
            _logger.LogInformation("Request was cancelled by the client");
        
            // Only modify response if it hasn't been sent yet
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = 499; // Client Closed Request
            }
        
            return;
        }
    }
}