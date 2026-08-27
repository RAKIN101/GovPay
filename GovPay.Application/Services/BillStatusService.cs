using GovPay.Application.Interfaces;

namespace GovPay.Application.Services;

public class BillStatusService
{
    private readonly IBillRepository _billRepository;

    public BillStatusService(IBillRepository billRepository)
    {
        _billRepository = billRepository;
    }

    public async Task UpdateOverdueBillsAsync()
    {
        var bills = await _billRepository.GetAllAsync();

        foreach (var bill in bills)
        {
            if (bill.Status == "Pending" && bill.DueDate < DateTime.UtcNow)
            {
                bill.Status = "Overdue";

                await _billRepository.UpdateAsync(bill);
            }
        }
    }
}