namespace TaskFlow.Application.Users.LoginUser;

public sealed record LoginUserResult(
    string Token,
    DateTime ExpiresAt
);
