namespace GovPay.Application.DTOs;

public class BillResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string BillNumber { get; set; } = string.Empty;

    public string BillType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }
}