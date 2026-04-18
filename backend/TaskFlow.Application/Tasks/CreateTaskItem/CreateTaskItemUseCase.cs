using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
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
        var title = command.Title.Trim();
        var description = command.Description?.Trim();

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            Status = TaskItemsStatus.Todo,
            ProjectId = command.ProjectId
        };

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