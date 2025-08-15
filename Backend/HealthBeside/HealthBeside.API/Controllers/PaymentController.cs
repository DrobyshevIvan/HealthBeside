using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Services;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Microsoft.AspNetCore.Components.Route("[controller]")]
[ApiController]

public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    // TODO : Реалізувати інші ендпоінти
    [HttpPut("process-payment")]
    [Authorize]
    public async Task<ActionResult> ProcessPayment([FromBody] ProcessPaymentRequest payment,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            // _logger.LogWarning("Unauthorized access attempt to create order");
            return Unauthorized();
        }
        
        await _paymentService.ProcessPayment(payment.PaymentId, payment.OrderId, userId, cancellationToken);
        
        return Ok();
    }
}