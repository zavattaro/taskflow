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

    public async Task<CreateProjectResult> ExecuteAsync(CreateProjectCommand command, CancellationToken ct = default)
    {
        var project = Project.Create(command.UserId, command.Name, command.Description);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(ct);

        return new CreateProjectResult(project.Id, project.Name, project.Description);
    }
}
