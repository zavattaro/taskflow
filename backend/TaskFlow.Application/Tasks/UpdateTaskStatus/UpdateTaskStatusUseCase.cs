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

    public async Task<UpdateTaskStatusResult> ExecuteAsync(UpdateTaskStatusCommand command)
    {
        var parsed = Enum.TryParse<TaskItemStatus>(
            command.Status,
            true,
            out var newStatus
        );

        if (!parsed)
            throw new ValidationException("Status", "Invalid status. Allowed values: Todo, Doing, Done.");

        var taskItem = await _context.Tasks
            .FirstOrDefaultAsync(x =>
                x.Id == command.TaskItemId &&
                x.ProjectId == command.ProjectId
            );

        if (taskItem is null)
            throw new NotFoundException("TaskItem", command.TaskItemId);

        // Delegação correta ao domínio
        taskItem.UpdateStatus(newStatus);

        await _context.SaveChangesAsync();

        return new UpdateTaskStatusResult(
            taskItem.Id,
            taskItem.Title,
            taskItem.Description,
            taskItem.Status.ToString(),
            taskItem.ProjectId
        );
    }
}
