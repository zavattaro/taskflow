namespace TaskFlow.Api.Contracts.Tasks;

public class TaskItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}
