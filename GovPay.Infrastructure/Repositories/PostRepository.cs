using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;
using GovPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GovPay.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly GovPayDbContext _context;

    public PostRepository(GovPayDbContext context)
    {
        _context = context;
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Post>> GetByUserIdAsync(int userId)
    {
        return await _context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Post>> GetAllAsync()
    {
        return await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }
}
