using System.Security.Claims;
using GovPay.Application.DTOs;
using GovPay.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly BillService _billService;

    public BillsController(BillService billService)
    {
        _billService = billService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyBills()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);
        var bills = await _billService.GetByUserIdAsync(userId);

        return Ok(bills.Select(ToResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBill(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim);
        var bill = await _billService.GetByIdAsync(id);

        if (bill is null)
        {
            return NotFound(new { message = "Bill not found." });
        }

        if (bill.UserId != userId)
        {
            return Forbid();
        }

        return Ok(ToResponse(bill));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBill(CreateBillRequest request)
    {
        var bill = await _billService.CreateAsync(request);

        return Ok(ToResponse(bill));
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBills()
    {
        var bills = await _billService.GetAllAsync();

        return Ok(bills.Select(ToResponse));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        var bill = await _billService.GetByIdAsync(id);

        if (bill is null)
        {
            return NotFound(new { message = "Bill not found." });
        }

        if (status != "Pending" && status != "Paid" && status != "Overdue")
        {
            return BadRequest(new { message = "Invalid bill status." });
        }

        bill.Status = status;

        if (status == "Paid")
        {
            bill.PaidAt = DateTime.UtcNow;
        }

        await _billService.UpdateAsync(bill);

        return Ok(ToResponse(bill));
    }

    private static BillResponse ToResponse(Domain.Entities.Bill bill)
    {
        return new BillResponse
        {
            Id = bill.Id,
            UserId = bill.UserId,
            BillNumber = bill.BillNumber,
            BillType = bill.BillType,
            Amount = bill.Amount,
            DueDate = bill.DueDate,
            Status = bill.Status,
            Description = bill.Description,
            CreatedAt = bill.CreatedAt,
            PaidAt = bill.PaidAt
        };
    }
}