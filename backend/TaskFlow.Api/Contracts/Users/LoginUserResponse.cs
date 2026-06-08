namespace TaskFlow.Api.Contracts.Users;

public sealed record LoginUserResponse(
    string Token,
    DateTime ExpiresAt
);
