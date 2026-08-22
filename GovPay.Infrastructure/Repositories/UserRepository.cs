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

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}