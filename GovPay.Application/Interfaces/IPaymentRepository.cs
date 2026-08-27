using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);

    Task<Payment?> GetByIdAsync(int id);

    Task<List<Payment>> GetByUserIdAsync(int userId);

    Task<bool> HasPaymentForBillAsync(int billId);
}