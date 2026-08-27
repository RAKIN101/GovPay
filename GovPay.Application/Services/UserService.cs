using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Cryptography.Hashing;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly TwoFactorService _twoFactorService;

    public UserService(
        IUserRepository userRepository,
        PasswordHasher passwordHasher,
        TwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _twoFactorService = twoFactorService;
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

        var isPasswordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash,
            user.PasswordSalt);

        if (!isPasswordValid)
        {
            return null;
        }

        if (user.TwoFactorEnabled)
        {
            var result = _twoFactorService.GenerateCode();

            user.TwoFactorCodeHash = result.Hash;
            user.TwoFactorCodeExpiresAt = result.ExpiresAt;

            await _userRepository.UpdateAsync(user);

            Console.WriteLine(
                $"[DEV] 2FA OTP for {user.Username}: {result.Code}");
        }

        return user;
    }
    public async Task<User?> VerifyTwoFactorAsync(VerifyTwoFactorRequest request)
{
    var user = await _userRepository.GetByUsernameAsync(request.Username);

    if (user is null)
    {
        return null;
    }

    if (!user.TwoFactorEnabled)
    {
        return null;
    }

    if (string.IsNullOrEmpty(user.TwoFactorCodeHash) ||
        string.IsNullOrEmpty(user.TwoFactorCodeSalt) ||
        user.TwoFactorCodeExpiresAt is null)
    {
        return null;
    }

    if (DateTime.UtcNow > user.TwoFactorCodeExpiresAt.Value)
    {
        return null;
    }

    var isCodeValid = _passwordHasher.VerifyPassword(
        request.Code,
        user.TwoFactorCodeHash,
        user.TwoFactorCodeSalt);

    if (!isCodeValid)
    {
        return null;
    }

    // OTP can only be used once
    user.TwoFactorCodeHash = null;
    user.TwoFactorCodeSalt = null;
    user.TwoFactorCodeExpiresAt = null;

    await _userRepository.UpdateAsync(user);

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

    public Task<User?> GetUserById(int id)
    {
        throw new NotImplementedException();
    }
}