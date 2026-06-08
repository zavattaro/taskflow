namespace TaskFlow.Domain.Exceptions;

public class ValidationException : DomainException
{
    public string Field { get; }

    public ValidationException(string field, string message)
        : base(message)
    {
        Field = field;
    }
}
