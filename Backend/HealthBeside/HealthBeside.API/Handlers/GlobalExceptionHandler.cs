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
        var (statusCode, body) = GetExceptionDetails(exception);
        
        _logger.LogError(exception, exception.Message);
        
        httpContext.Response.StatusCode = (int)statusCode;
        
        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);

        return true;
    }
    
    private (HttpStatusCode statusCode, object body) GetExceptionDetails(Exception exception)
    {
        switch (exception)
        {
            case FluentValidation.ValidationException fv:
                var errors = fv.Errors
                    .GroupBy(e => e.PropertyName ?? string.Empty)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

                return (HttpStatusCode.BadRequest, new
                {
                    type = "https://tools.ietf.org/html/rfc9119#section-15.5.1",
                    title = "One or more validation errors occured.",
                    status = 400,
                    errors
                });

            case LoginFailedException:
                return (HttpStatusCode.Unauthorized, exception.Message);

            case UserAlreadyExistsException:
                return (HttpStatusCode.Conflict, exception.Message);

            case UserRegistrationFailedException:
                return (HttpStatusCode.BadRequest, exception.Message);

            case RefreshTokenException:
                return (HttpStatusCode.Unauthorized, exception.Message);

            case ForumPostCreationException:
                return (HttpStatusCode.BadRequest, exception.Message);

            default:
                return (HttpStatusCode.InternalServerError, exception.Message);

        }
    }
}