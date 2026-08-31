using GovPay.Application.DTOs;
using GovPay.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var post = await _postService.CreateAsync(request);
        return Ok(post);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postService.GetAllAsync();
        return Ok(posts);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post is null) return NotFound();
        return Ok(post);
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var posts = await _postService.GetByUserIdAsync(userId);
        return Ok(posts);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request)
    {
        var post = await _postService.UpdateAsync(id, request);
        if (post is null) return NotFound();
        return Ok(post);
    }
}
