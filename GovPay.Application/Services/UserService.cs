using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
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