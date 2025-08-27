using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.Payment;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Services;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Microsoft.AspNetCore.Components.Route("[controller]")]
[ApiController]

public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
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

    [HttpGet("get-payment/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetDetailedPaymentDto>> GetPayment([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment for id {Id}", id);
        var payment =  await _paymentService.GetPayment(id, cancellationToken);
        return Ok(payment);
    }

    [HttpGet("get-payments")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<GetPaymentDto>>> GetAllPayments(
        [FromQuery] PaymentFilter paymentFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /get-payments called with filters: {@Filters}, sort: {@Sort}, page: {@Page}",
            paymentFilter, sortParams, pageParams);

        var payments = await _paymentService.GetPayments(paymentFilter, sortParams, pageParams, cancellationToken);
        _logger.LogInformation("Returned {Count} payments.", payments.Count());
        return Ok(payments);
    }
}