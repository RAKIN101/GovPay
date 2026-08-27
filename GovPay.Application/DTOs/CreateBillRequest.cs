namespace GovPay.Application.DTOs;

public class CreateBillRequest
{
    public int UserId { get; set; }

    public string BillNumber { get; set; } = string.Empty;

    public string BillType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string Description { get; set; } = string.Empty;
}