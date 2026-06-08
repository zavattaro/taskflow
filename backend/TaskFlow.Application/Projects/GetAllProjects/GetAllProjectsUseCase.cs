using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Projects.GetAllProjects;

public sealed class GetAllProjectsUseCase
{
    private readonly AppDbContext _context;

    public GetAllProjectsUseCase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProjectDto>> ExecuteAsync(GetAllProjectsQuery query, CancellationToken ct = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId)
            .OrderBy(x => x.Name)
            .Select(x => new ProjectDto(x.Id, x.Name, x.Description))
            .ToListAsync(ct);
    }
}
