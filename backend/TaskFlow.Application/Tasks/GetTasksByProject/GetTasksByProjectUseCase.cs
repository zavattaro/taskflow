using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Tasks.GetTasksByProject;

public sealed class GetTasksByProjectUseCase
{
    private readonly AppDbContext _context;

    public GetTasksByProjectUseCase(AppDbContext context)
    {
        _context = context;
    }

    public sealed record TaskItemDto(
        Guid Id,
        string Title,
        string? Description,
        string Status,
        Guid ProjectId
    );

    public async Task<IReadOnlyList<TaskItemDto>> ExecuteAsync(GetTasksByProjectQuery query, CancellationToken ct = default)
    {
        var projectOwned = await _context.Projects
            .AnyAsync(p => p.Id == query.ProjectId && p.UserId == query.UserId, ct);

        if (!projectOwned)
            throw new NotFoundException("Project", query.ProjectId);

        return await _context.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == query.ProjectId)
            .OrderBy(x => x.Title)
            .Select(x => new TaskItemDto(x.Id, x.Title, x.Description, x.Status.ToString(), x.ProjectId))
            .ToListAsync(ct);
    }
}
