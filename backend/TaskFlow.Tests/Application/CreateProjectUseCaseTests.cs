using TaskFlow.Application.Projects.CreateProject;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Application;

public class CreateProjectUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldCreateProjectAndReturnResult()
    {
        using var context = TestDbContextFactory.Create();
        var useCase = new CreateProjectUseCase(context);
        var userId = Guid.NewGuid();

        var result = await useCase.ExecuteAsync(
            new CreateProjectCommand(userId, "TaskFlow", "App de tarefas"));

        Assert.NotEqual(Guid.Empty, result.ProjectId);
        Assert.Equal("TaskFlow", result.Name);
        Assert.Equal("App de tarefas", result.Description);

        var saved = await context.Projects.FindAsync(result.ProjectId);
        Assert.NotNull(saved);
        Assert.Equal(userId, saved.UserId);
    }
}
