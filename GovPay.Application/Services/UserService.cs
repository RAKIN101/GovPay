using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Cryptography.Hashing;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        PasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = "Citizen",
            TwoFactorEnabled = false
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null)
        {
            return null;
        }

        var validPassword = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash,
            user.PasswordSalt);

        if (!validPassword)
        {
            return null;
        }

        return user;
    }

    public User CreateUser(User user)
    {
        return user;
    }

    public User? UpdateUser(User user)
    {
        return user;
    }
}