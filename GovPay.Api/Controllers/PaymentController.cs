using System.Security.Claims;
using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Pay(PaymentRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var payment = await _paymentService.PayBillAsync(userId.Value, request);

        if (payment is null)
        {
            return BadRequest(new
            {
                message = "Payment could not be processed."
            });
        }

        return Ok(payment);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var payments = await _paymentService.GetPaymentHistoryAsync(userId.Value);

        return Ok(payments);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();

        return Ok(payments);
    }

    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetById(int paymentId)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var payment = await _paymentService.GetPaymentByIdAsync(userId.Value, paymentId);

        if (payment is null)
        {
            return NotFound(new
            {
                message = "Payment not found."
            });
        }

        return Ok(payment);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}