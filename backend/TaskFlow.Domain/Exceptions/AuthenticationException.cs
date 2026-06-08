namespace TaskFlow.Domain.Exceptions;

public class AuthenticationException : DomainException
{
    public AuthenticationException() : base("Invalid credentials.") { }
}
