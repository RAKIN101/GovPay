using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);

    Task<User?> GetByIdAsync(int id);

    Task<List<User>> GetAllAsync();

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> DeleteAsync(int id);

    Task UpdateAsync(User user);
}