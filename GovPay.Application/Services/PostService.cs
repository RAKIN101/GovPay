using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
using GovPay.Domain.Entities;

namespace GovPay.Application.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post> CreateAsync(CreatePostRequest request)
    {
        var post = new Post
        {
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _postRepository.CreateAsync(post);
    }

    public async Task<List<Post>> GetByUserIdAsync(int userId)
    {
        return await _postRepository.GetByUserIdAsync(userId);
    }

    public async Task<List<Post>> GetAllAsync()
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _postRepository.GetByIdAsync(id);
    }

    public async Task<Post?> UpdateAsync(int id, UpdatePostRequest request)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post is null)
        {
            return null;
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;

        await _postRepository.UpdateAsync(post);
        return post;
    }
}
