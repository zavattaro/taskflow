namespace TaskFlow.Application.Projects.GetAllProjects;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description
);
