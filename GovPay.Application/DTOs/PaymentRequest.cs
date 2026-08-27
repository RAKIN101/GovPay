namespace GovPay.Application.DTOs;

public class PaymentRequest
{
    public int BillId { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
}