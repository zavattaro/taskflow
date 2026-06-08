namespace TaskFlow.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password
);
