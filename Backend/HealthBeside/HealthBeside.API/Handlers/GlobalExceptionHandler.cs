using System.Net;
using HealthBeside.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace HealthBeside.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = GetExceptionDetails(exception);
        
        _logger.LogError(exception, exception.Message);
        
        httpContext.Response.StatusCode = (int)statusCode;
        
        await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

        return true;
    }
    
    private (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
    {
        return exception switch 
        {
            LoginFailedException => (HttpStatusCode.Unauthorized, exception.Message),
            UserAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            UserRegistrationFailedException => (HttpStatusCode.BadRequest, exception.Message),
            RefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message),
            ForumPostCreationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, exception.Message) 
        };
    }
}