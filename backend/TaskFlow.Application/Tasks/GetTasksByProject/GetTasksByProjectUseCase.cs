using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Tasks.GetTasksByProject;

public sealed class GetTasksByProjectUseCase
{
    private readonly AppDbContext _context;

    public GetTasksByProjectUseCase(AppDbContext context)
    {
        _context = context;
    }
    public sealed class TaskItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = default!;
        public string? Description { get; init; }
        public string Status { get; init; } = default!;
        public Guid ProjectId { get; init; }
    }


    public async Task<IReadOnlyList<TaskItemDto>> ExecuteAsync(GetTasksByProjectQuery query)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == query.ProjectId)
            .OrderBy(x => x.Title)
            .Select(x => new TaskItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status.ToString(),
                ProjectId = x.ProjectId
            })
            .ToListAsync();
    }
}