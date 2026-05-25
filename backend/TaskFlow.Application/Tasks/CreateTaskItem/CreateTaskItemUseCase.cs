using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Tasks.CreateTaskItem;

public sealed class CreateTaskItemUseCase
{
    private readonly AppDbContext _context;

    public CreateTaskItemUseCase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateTaskItemResult> ExecuteAsync(CreateTaskItemCommand command)
    {
        var taskItem = TaskItem.Create(
            command.ProjectId,
            command.Title,
            command.Description
        );

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync();

        return new CreateTaskItemResult(
            taskItem.Id,
            taskItem.Title,
            taskItem.Description,
            taskItem.Status.ToString(),
            taskItem.ProjectId
        );
    }
}