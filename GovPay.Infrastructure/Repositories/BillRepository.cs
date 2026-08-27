using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;
using GovPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GovPay.Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly GovPayDbContext _context;

    public BillRepository(GovPayDbContext context)
    {
        _context = context;
    }

    public async Task<Bill> CreateAsync(Bill bill)
    {
        _context.Bills.Add(bill);

        await _context.SaveChangesAsync();

        return bill;
    }

    public async Task<Bill?> GetByIdAsync(int id)
    {
        return await _context.Bills
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Bill>> GetByUserIdAsync(int userId)
    {
        return await _context.Bills
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Bill>> GetAllAsync()
    {
        return await _context.Bills
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Bill bill)
    {
        _context.Bills.Update(bill);

        await _context.SaveChangesAsync();
    }
}