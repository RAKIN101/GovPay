namespace GovPay.Application.DTOs;

public class CreatePostRequest
{
    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
