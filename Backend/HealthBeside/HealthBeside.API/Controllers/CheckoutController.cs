using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Interfaces;
using HealthBeside.Infrastructure.Configurations.MarketConfiguration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly IStripeService _stripeService;
    private readonly IOptions<StripeSettings> _stripeSettings;

    public CheckoutController(ILogger<CheckoutController> logger,
        IStripeService stripeService,
        IOptions<StripeSettings> stripeSettings)
    {
        _logger = logger;
        _stripeService = stripeService;
        _stripeSettings = stripeSettings;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("");
            return Unauthorized("Invalid or missing user identifier.");
        }

        var sessionUrl = await _stripeService.CreateSession(model.OrderId, userId);
        return Ok(sessionUrl);
    }

    [HttpPost("~/webhook")]
    public async Task<IActionResult> WebHook(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("=== WebHook START ===");
        _logger.LogInformation("Request ID: {RequestId}", HttpContext.TraceIdentifier);
        
        Request.EnableBuffering();
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;
        
        try
        {
            await _stripeService.ProcessPayment(json, Request, CancellationToken.None);
            _logger.LogInformation("=== WebHook END ===");
            return Ok();
        }
        catch (StripeException e)
        {
            var errorMessage = e.StripeError?.Message ?? e.Message ?? "Unknown Stripe error";
            _logger.LogError("Stripe error: {Error}", errorMessage);
            return BadRequest(new { error = errorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in webhook");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}