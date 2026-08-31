using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post?> GetByIdAsync(int id);
    Task<List<Post>> GetByUserIdAsync(int userId);
    Task<List<Post>> GetAllAsync();
    Task UpdateAsync(Post post);
}
