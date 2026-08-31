using GovPay.Application.DTOs;

namespace GovPay.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse?> PayBillAsync(
        int userId,
        PaymentRequest request);

    Task<List<PaymentResponse>> GetPaymentHistoryAsync(
        int userId);

    Task<List<PaymentResponse>> GetAllPaymentsAsync();

    Task<PaymentResponse?> GetPaymentByIdAsync(
        int userId,
        int paymentId);
}