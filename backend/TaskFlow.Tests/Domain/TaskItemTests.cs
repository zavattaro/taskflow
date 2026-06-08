using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Tests.Domain;

public class TaskItemTests
{
    private readonly Guid _projectId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldReturnTaskItem()
    {
        var task = TaskItem.Create(_projectId, "Implementar login", "JWT auth");

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("Implementar login", task.Title);
        Assert.Equal("JWT auth", task.Description);
        Assert.Equal(TaskItemStatus.Todo, task.Status);
        Assert.Equal(_projectId, task.ProjectId);
    }

    [Fact]
    public void Create_ShouldDefaultStatusToTodo()
    {
        var task = TaskItem.Create(_projectId, "Task", null);

        Assert.Equal(TaskItemStatus.Todo, task.Status);
    }

    [Fact]
    public void Create_WithEmptyProjectId_ShouldThrow()
    {
        Assert.Throws<ValidationException>(() =>
            TaskItem.Create(Guid.Empty, "Task", null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ShouldThrow(string? title)
    {
        Assert.Throws<ValidationException>(() =>
            TaskItem.Create(_projectId, title!, null));
    }

    [Theory]
    [InlineData(TaskItemStatus.Doing)]
    [InlineData(TaskItemStatus.Done)]
    [InlineData(TaskItemStatus.Todo)]
    public void UpdateStatus_ShouldChangeStatus(TaskItemStatus newStatus)
    {
        var task = TaskItem.Create(_projectId, "Task", null);

        task.UpdateStatus(newStatus);

        Assert.Equal(newStatus, task.Status);
    }

    [Fact]
    public void Update_ShouldChangeTitleAndDescription()
    {
        var task = TaskItem.Create(_projectId, "Old", "Old desc");

        task.Update("New", "New desc");

        Assert.Equal("New", task.Title);
        Assert.Equal("New desc", task.Description);
    }

    [Fact]
    public void Update_WithEmptyTitle_ShouldThrow()
    {
        var task = TaskItem.Create(_projectId, "Task", null);

        Assert.Throws<ValidationException>(() => task.Update("", null));
    }
}
