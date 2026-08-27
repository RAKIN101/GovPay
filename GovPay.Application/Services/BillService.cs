using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class BillService
{
    private readonly IBillRepository _billRepository;

    public BillService(IBillRepository billRepository)
    {
        _billRepository = billRepository;
    }

    public async Task<Bill> CreateAsync(CreateBillRequest request)
    {
        var bill = new Bill
        {
            UserId = request.UserId,
            BillNumber = request.BillNumber,
            BillType = request.BillType,
            Amount = request.Amount,
            DueDate = request.DueDate,
            Description = request.Description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        return await _billRepository.CreateAsync(bill);
    }

    public async Task<Bill?> GetByIdAsync(int id)
    {
        return await _billRepository.GetByIdAsync(id);
    }

    public async Task<List<Bill>> GetByUserIdAsync(int userId)
    {
        return await _billRepository.GetByUserIdAsync(userId);
    }

    public async Task<List<Bill>> GetAllAsync()
    {
        return await _billRepository.GetAllAsync();
    }

    public async Task UpdateAsync(Bill bill)
    {
        await _billRepository.UpdateAsync(bill);
    }
}