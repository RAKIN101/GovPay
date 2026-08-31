using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;
using GovPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GovPay.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GovPayDbContext _context;

    public UserRepository(GovPayDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return null;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}