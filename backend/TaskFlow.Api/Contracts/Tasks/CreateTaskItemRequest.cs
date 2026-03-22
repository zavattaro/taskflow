namespace TaskFlow.Api.Contracts.Tasks;

public class CreateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
