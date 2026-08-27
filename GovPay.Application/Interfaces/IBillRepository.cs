using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IBillRepository
{
    Task<Bill> CreateAsync(Bill bill);

    Task<Bill?> GetByIdAsync(int id);

    Task<List<Bill>> GetByUserIdAsync(int userId);

    Task<List<Bill>> GetAllAsync();

    Task UpdateAsync(Bill bill);
}