namespace TaskFlow.Api.Contracts.Tasks;

public sealed record TaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    Guid ProjectId
);
