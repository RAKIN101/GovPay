using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;
using GovPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GovPay.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly GovPayDbContext _context;

    public PaymentRepository(GovPayDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.Bill)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Payment>> GetByUserIdAsync(int userId)
    {
        return await _context.Payments
            .Include(p => p.Bill)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Bill)
            .Include(p => p.User)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();
    }

    public async Task<bool> HasPaymentForBillAsync(int billId)
    {
        return await _context.Payments
            .AnyAsync(p => p.BillId == billId);
    }
}