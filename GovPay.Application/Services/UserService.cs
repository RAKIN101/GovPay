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
    private readonly IJwtService _jwtService;

    public UserService(
        IUserRepository userRepository,
        PasswordHasher passwordHasher,
        TwoFactorService twoFactorService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _twoFactorService = twoFactorService;
        _jwtService = jwtService;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return null;
        }

        return await _userRepository.DeleteAsync(id);
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var (hash, salt) = _passwordHasher.HashPassword(request.Password);

        var normalizedRole = string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Citizen";

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = normalizedRole,
            TwoFactorEnabled = false
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
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
            var otpResult = _twoFactorService.GenerateCode();

            user.TwoFactorCodeHash = otpResult.Hash;
            user.TwoFactorCodeSalt = otpResult.Salt;
            user.TwoFactorCodeExpiresAt = otpResult.ExpiresAt;

            await _userRepository.UpdateAsync(user);

            Console.WriteLine(
                $"[DEV] 2FA OTP for {user.Username}: {otpResult.Code}");

            return new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                RequiresTwoFactor = true,
                Token = null
            };
        }

        return new LoginResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            RequiresTwoFactor = false,
            Token = _jwtService.GenerateToken(user)
        };
    }
    public async Task<LoginResponse?> VerifyTwoFactorAsync(VerifyTwoFactorRequest request)
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

        user.TwoFactorCodeHash = null;
        user.TwoFactorCodeSalt = null;
        user.TwoFactorCodeExpiresAt = null;

        await _userRepository.UpdateAsync(user);

        return new LoginResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            RequiresTwoFactor = false,
            Token = _jwtService.GenerateToken(user)
        };
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