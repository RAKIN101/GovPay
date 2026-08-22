using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
}