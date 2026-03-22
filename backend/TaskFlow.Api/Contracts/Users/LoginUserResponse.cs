namespace TaskFlow.Api.Contracts.Users;

public class LoginUserResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
