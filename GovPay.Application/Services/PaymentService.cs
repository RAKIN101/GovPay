using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBillRepository _billRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IBillRepository billRepository)
    {
        _paymentRepository = paymentRepository;
        _billRepository = billRepository;
    }

    public async Task<PaymentResponse?> PayBillAsync(
        int userId,
        PaymentRequest request)
    {
        var bill = await _billRepository.GetByIdAsync(request.BillId);

        if (bill is null)
        {
            return null;
        }

        if (bill.UserId != userId)
        {
            return null;
        }

        if (bill.Status == "Paid")
        {
            return null;
        }

        var alreadyPaid = await _paymentRepository.HasPaymentForBillAsync(request.BillId);

        if (alreadyPaid)
        {
            return null;
        }

        var transactionReference = $"GP-{Guid.NewGuid():N}".ToUpper();

        var payment = new Payment
        {
            BillId = bill.Id,
            UserId = userId,
            Amount = bill.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = "Completed",
            PaidAt = DateTime.UtcNow,
            TransactionReference = transactionReference
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);

        bill.Status = "Paid";
        bill.PaidAt = DateTime.UtcNow;

        await _billRepository.UpdateAsync(bill);

        return ToResponse(createdPayment);
    }

    public async Task<List<PaymentResponse>> GetPaymentHistoryAsync(int userId)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(userId);

        return payments.Select(ToResponse).ToList();
    }

    public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();

        return payments.Select(ToResponse).ToList();
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(int userId, int paymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment is null || payment.UserId != userId)
        {
            return null;
        }

        return ToResponse(payment);
    }

    private static PaymentResponse ToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            UserId = payment.UserId,
            BillId = payment.BillId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            TransactionReference = payment.TransactionReference
        };
    }
}