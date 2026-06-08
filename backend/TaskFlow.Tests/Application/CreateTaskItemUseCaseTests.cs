using TaskFlow.Application.Tasks.CreateTaskItem;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Application;

public class CreateTaskItemUseCaseTests
{
    [Fact]
    public async Task Execute_WithValidProject_ShouldCreateTask()
    {
        using var context = TestDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var project = Project.Create(userId, "Projeto", null);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var useCase = new CreateTaskItemUseCase(context);

        var result = await useCase.ExecuteAsync(
            new CreateTaskItemCommand(project.Id, userId, "Nova task", "Desc"));

        Assert.NotEqual(Guid.Empty, result.TaskItemId);
        Assert.Equal("Nova task", result.Title);
        Assert.Equal("Todo", result.Status);
    }

    [Fact]
    public async Task Execute_WithInvalidProject_ShouldThrowNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var useCase = new CreateTaskItemUseCase(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(
                new CreateTaskItemCommand(Guid.NewGuid(), Guid.NewGuid(), "Task", null)));
    }

    [Fact]
    public async Task Execute_WithWrongUser_ShouldThrowNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var ownerId = Guid.NewGuid();
        var project = Project.Create(ownerId, "Projeto", null);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var useCase = new CreateTaskItemUseCase(context);
        var intruderId = Guid.NewGuid();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(
                new CreateTaskItemCommand(project.Id, intruderId, "Task", null)));
    }
}
