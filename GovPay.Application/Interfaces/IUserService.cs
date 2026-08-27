using GovPay.Application.DTOs;
using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);

    Task<User> RegisterAsync(RegisterRequest request);

    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> VerifyTwoFactorAsync(VerifyTwoFactorRequest request);

    User CreateUser(User user);

    User? UpdateUser(User user);
}