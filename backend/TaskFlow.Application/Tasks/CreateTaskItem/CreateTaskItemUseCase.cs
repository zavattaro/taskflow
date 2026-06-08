using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Tasks.CreateTaskItem;

public sealed class CreateTaskItemUseCase
{
    private readonly AppDbContext _context;

    public CreateTaskItemUseCase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTaskItemResult> ExecuteAsync(CreateTaskItemCommand command, CancellationToken ct = default)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == command.ProjectId && p.UserId == command.UserId, ct);

        if (!projectExists)
            throw new NotFoundException("Project", command.ProjectId);

        var taskItem = TaskItem.Create(command.ProjectId, command.Title, command.Description);

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync(ct);

        return new CreateTaskItemResult(
            taskItem.Id, taskItem.Title, taskItem.Description,
            taskItem.Status.ToString(), taskItem.ProjectId
        );
    }
}
