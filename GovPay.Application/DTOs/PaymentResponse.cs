namespace GovPay.Application.DTOs;

public class PaymentResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BillId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime PaidAt { get; set; }

    public string TransactionReference { get; set; } = string.Empty;
}