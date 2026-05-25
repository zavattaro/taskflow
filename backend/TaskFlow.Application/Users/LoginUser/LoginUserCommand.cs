namespace TaskFlow.Application.Users.LoginUser;

public sealed class LoginUserCommand
{
    public string Email { get; }
    public string Password { get; }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}