namespace TaskFlow.Api.Contracts.Users;

public sealed record RegisterUserResponse(
    Guid Id,
    string Name,
    string Email
);
