using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Application.Services;
using GovPay.Cryptography.Hashing;
using GovPay.Domain.Entities;

namespace GovPay.Cryptography.Tests;

public class UserServiceDeleteTests
{
    [Fact]
    public async Task DeleteUserAsync_RemovesExistingUser()
    {
        var repository = new FakeUserRepository();
        var hasher = new PasswordHasher();
        var service = new UserService(repository, hasher, new TwoFactorService(hasher), new FakeJwtService());

        var user = new User
        {
            Id = 7,
            Username = "alice",
            Email = "alice@govpay.com",
            PasswordHash = "hash",
            PasswordSalt = "salt",
            Role = "Citizen"
        };

        await repository.CreateAsync(user);

        var removed = await service.DeleteUserAsync(7);

        Assert.NotNull(removed);
        Assert.Equal(7, removed.Id);
        Assert.Empty(repository.Users);
    }

    private class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();

        public Task<User> CreateAsync(User user)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
        }

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(Users.ToList());
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Username == username));
        }

        public Task UpdateAsync(User user)
        {
            var existing = Users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                var index = Users.IndexOf(existing);
                Users[index] = user;
            }

            return Task.CompletedTask;
        }

        public Task<User?> DeleteAsync(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                Users.Remove(user);
            }

            return Task.FromResult(user);
        }
    }

    private class FakeJwtService : IJwtService
    {
        public string GenerateToken(User user)
        {
            return $"token-for-{user.Username}";
        }
    }
}
