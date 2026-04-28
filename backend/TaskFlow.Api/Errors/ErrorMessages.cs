namespace TaskFlow.Api.Errors;

public static class ErrorMessages
{
    public const string InvalidUserContext = "Invalid user context.";
    public const string ProjectNotFound = "Project not found.";
    public const string TaskNotFound = "Task not found.";
    public const string StatusRequired = "Status is required.";
    public const string InvalidStatus = "Invalid status. Allowed values: Todo, Doing, Done.";
    public const string TitleRequired = "Title is required.";
    public const string UnexpectedError = "An unexpected error occurred.";
    public const string NameRequired = "Name is required.";
    public const string EmailRequired = "Email is required.";
    public const string EmailAlreadyRegistered = "Email is already registered.";
    public const string PasswordRequired = "Password is required.";
    public const string InvalidCredentials = "Invalid credentials.";
    public const string JwtKeyNotConfigured = "JWT key not configured.";
    public const string JwtIssuerNotConfigured = "JWT issuer not configured.";
    public const string JwtAudienceNotConfigured = "JWT audience not configured.";

}
