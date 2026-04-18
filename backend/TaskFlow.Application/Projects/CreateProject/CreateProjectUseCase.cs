using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Projects.CreateProject;

public sealed class CreateProjectUseCase
{
    private readonly AppDbContext _context;

    public CreateProjectUseCase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProjectResult> ExecuteAsync(CreateProjectCommand command)
    {
        var name = command.Name.Trim();
        var description = command.Description?.Trim();

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            UserId = command.UserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return new CreateProjectResult(
            project.Id,
            project.Name,
            project.Description
        );
    }
}