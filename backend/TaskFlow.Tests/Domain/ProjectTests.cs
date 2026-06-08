using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Tests.Domain;

public class ProjectTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldReturnProject()
    {
        var project = Project.Create(_userId, "TaskFlow", "App de tarefas");

        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal("TaskFlow", project.Name);
        Assert.Equal("App de tarefas", project.Description);
        Assert.Equal(_userId, project.UserId);
    }

    [Fact]
    public void Create_WithNullDescription_ShouldSetNull()
    {
        var project = Project.Create(_userId, "TaskFlow", null);

        Assert.Null(project.Description);
    }

    [Fact]
    public void Create_WithWhitespaceDescription_ShouldSetNull()
    {
        var project = Project.Create(_userId, "TaskFlow", "   ");

        Assert.Null(project.Description);
    }

    [Fact]
    public void Create_ShouldTrimName()
    {
        var project = Project.Create(_userId, "  TaskFlow  ", null);

        Assert.Equal("TaskFlow", project.Name);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrow()
    {
        Assert.Throws<ValidationException>(() =>
            Project.Create(Guid.Empty, "TaskFlow", null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        Assert.Throws<ValidationException>(() =>
            Project.Create(_userId, name!, null));
    }

    [Fact]
    public void Update_ShouldChangeNameAndDescription()
    {
        var project = Project.Create(_userId, "Old Name", "Old desc");

        project.Update("New Name", "New desc");

        Assert.Equal("New Name", project.Name);
        Assert.Equal("New desc", project.Description);
    }

    [Fact]
    public void Update_WithEmptyName_ShouldThrow()
    {
        var project = Project.Create(_userId, "TaskFlow", null);

        Assert.Throws<ValidationException>(() => project.Update("", null));
    }
}
