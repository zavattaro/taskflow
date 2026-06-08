namespace TaskFlow.Domain.Exceptions;

public class AccessDeniedException : DomainException
{
    public AccessDeniedException(string entity, Guid id)
        : base($"Access denied to {entity} with id '{id}'.") { }
}
