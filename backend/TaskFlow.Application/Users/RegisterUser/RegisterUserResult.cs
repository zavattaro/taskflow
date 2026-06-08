namespace TaskFlow.Application.Users.RegisterUser;

public sealed record RegisterUserResult(
    Guid UserId,
    string Name,
    string Email
);
