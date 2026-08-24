using GovPay.Application.DTOs;
using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);

    Task<User> RegisterAsync(RegisterRequest request);

    Task<User?> LoginAsync(LoginRequest request);

    User CreateUser(User user);

    User? UpdateUser(User user);
}