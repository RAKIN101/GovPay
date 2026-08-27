namespace GovPay.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int BillId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = "Completed";

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public string TransactionReference { get; set; } = string.Empty;

    public Bill? Bill { get; set; }

    public User? User { get; set; }
}