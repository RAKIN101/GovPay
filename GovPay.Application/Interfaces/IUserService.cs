using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserById(int id);

    User CreateUser(User user);

    User? UpdateUser(User user);
    Task<User?> GetUserByIdAsync(int id);
}