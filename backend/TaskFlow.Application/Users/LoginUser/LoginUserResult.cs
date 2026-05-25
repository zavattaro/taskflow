namespace TaskFlow.Application.Users.LoginUser;

public sealed class LoginUserResult
{
    public string Token { get; }
    public DateTime ExpiresAt { get; }

    public LoginUserResult(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }
}