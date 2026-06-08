using TaskFlow.Application.Tasks.UpdateTaskStatus;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Application;

public class UpdateTaskStatusUseCaseTests
{
    private async Task<(Guid userId, Guid projectId, Guid taskId)> SeedAsync(Infrastructure.Persistence.AppDbContext context)
    {
        var userId = Guid.NewGuid();
        var project = Project.Create(userId, "Projeto", null);
        var task = TaskItem.Create(project.Id, "Task", null);

        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        return (userId, project.Id, task.Id);
    }

    [Fact]
    public async Task Execute_WithValidStatus_ShouldUpdate()
    {
        using var context = TestDbContextFactory.Create();
        var (userId, projectId, taskId) = await SeedAsync(context);
        var useCase = new UpdateTaskStatusUseCase(context);

        var result = await useCase.ExecuteAsync(
            new UpdateTaskStatusCommand(projectId, userId, taskId, "Doing"));

        Assert.Equal("Doing", result.Status);
    }

    [Fact]
    public async Task Execute_WithInvalidStatus_ShouldThrowValidation()
    {
        using var context = TestDbContextFactory.Create();
        var (userId, projectId, taskId) = await SeedAsync(context);
        var useCase = new UpdateTaskStatusUseCase(context);

        await Assert.ThrowsAsync<ValidationException>(() =>
            useCase.ExecuteAsync(
                new UpdateTaskStatusCommand(projectId, userId, taskId, "Invalid")));
    }

    [Fact]
    public async Task Execute_WithNonExistentTask_ShouldThrowNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var (userId, projectId, _) = await SeedAsync(context);
        var useCase = new UpdateTaskStatusUseCase(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(
                new UpdateTaskStatusCommand(projectId, userId, Guid.NewGuid(), "Done")));
    }

    [Fact]
    public async Task Execute_WithWrongUser_ShouldThrowNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var (_, projectId, taskId) = await SeedAsync(context);
        var useCase = new UpdateTaskStatusUseCase(context);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            useCase.ExecuteAsync(
                new UpdateTaskStatusCommand(projectId, Guid.NewGuid(), taskId, "Done")));
    }
}
