using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Tasks.UpdateTaskStatus;

public sealed class UpdateTaskStatusUseCase
{
    private readonly AppDbContext _context;

    public UpdateTaskStatusUseCase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateTaskStatusResult> ExecuteAsync(UpdateTaskStatusCommand command, CancellationToken ct = default)
    {
        var parsed = Enum.TryParse<TaskItemStatus>(command.Status, true, out var newStatus);

        if (!parsed)
            throw new ValidationException("Status", "Invalid status. Allowed values: Todo, Doing, Done.");

        var projectOwned = await _context.Projects
            .AnyAsync(p => p.Id == command.ProjectId && p.UserId == command.UserId, ct);

        if (!projectOwned)
            throw new NotFoundException("Project", command.ProjectId);

        var taskItem = await _context.Tasks
            .FirstOrDefaultAsync(x => x.Id == command.TaskItemId && x.ProjectId == command.ProjectId, ct);

        if (taskItem is null)
            throw new NotFoundException("TaskItem", command.TaskItemId);

        taskItem.UpdateStatus(newStatus);
        await _context.SaveChangesAsync(ct);

        return new UpdateTaskStatusResult(
            taskItem.Id, taskItem.Title, taskItem.Description,
            taskItem.Status.ToString(), taskItem.ProjectId
        );
    }
}
