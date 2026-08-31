using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class UserProfileService
{
    private readonly IUserRepository _userRepository;

    public UserProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetProfileAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> UpdateProfileAsync(int userId, ProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        user.Email = request.Email ?? user.Email;
        user.Username = user.Username;

        await _userRepository.UpdateAsync(user);
        return user;
    }
}
