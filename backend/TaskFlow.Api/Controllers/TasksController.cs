using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Contracts.Tasks;
using TaskFlow.Api.Controllers.Base;
using TaskFlow.Api.Errors;
using TaskFlow.Application.Tasks.CreateTaskItem;
using TaskFlow.Application.Tasks.GetTasksByProject;
using TaskFlow.Application.Tasks.UpdateTaskStatus;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController : AuthenticatedControllerBase
{
    private readonly AppDbContext _context;
    private readonly CreateTaskItemUseCase _createTaskItemUseCase;
    private readonly UpdateTaskStatusUseCase _updateTaskStatusUseCase;
    private readonly GetTasksByProjectUseCase _getTasksByProjectUseCase;

    public TasksController(AppDbContext context, CreateTaskItemUseCase createTaskItemUseCase, UpdateTaskStatusUseCase updateTaskStatusUseCase, GetTasksByProjectUseCase getTasksByProjectUseCase)
    {
        _context = context;
        _createTaskItemUseCase = createTaskItemUseCase;
        _updateTaskStatusUseCase = updateTaskStatusUseCase;
        _getTasksByProjectUseCase = getTasksByProjectUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskItemRequest request)
    {
        var title = request.Title?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new { message = ErrorMessages.TitleRequired });

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new { message = ErrorMessages.InvalidUserContext });

        var projectExists = await ProjectBelongsToUserAsync(projectId, userId);

        if (!projectExists)
            return NotFound(new { message = ErrorMessages.ProjectNotFound });

        var command = new CreateTaskItemCommand(
            projectId,
            userId,
            title,
            description
        );

        var result = await _createTaskItemUseCase.ExecuteAsync(command);

        var response = new TaskItemResponse
        {
            Id = result.TaskItemId,
            Title = result.Title,
            Description = result.Description,
            Status = result.Status,
            ProjectId = result.ProjectId
        };

        return Created(
            $"/api/projects/{projectId}/tasks/{result.TaskItemId}",
            response
        );
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid projectId, Guid taskId, UpdateTaskItemStatusRequest request)
    {
        var statusValue = request.Status?.Trim();

        if (string.IsNullOrWhiteSpace(statusValue))
            return BadRequest(new { message = ErrorMessages.StatusRequired });

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new { message = ErrorMessages.InvalidUserContext });

        var projectExists = await ProjectBelongsToUserAsync(projectId, userId);

        if (!projectExists)
            return NotFound(new { message = ErrorMessages.ProjectNotFound });

        var command = new UpdateTaskStatusCommand(projectId, taskId, statusValue);

        var result = await _updateTaskStatusUseCase.ExecuteAsync(command);

        if (result is null)
            return BadRequest(new
            {
                message = ErrorMessages.InvalidStatus
            });

        var response = new TaskItemResponse
        {
            Id = result.TaskItemId,
            Title = result.Title,
            Description = result.Description,
            Status = result.Status,
            ProjectId = result.ProjectId
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid projectId)
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new { message = ErrorMessages.InvalidUserContext });

        var projectExists = await ProjectBelongsToUserAsync(projectId, userId);

        if (!projectExists)
            return NotFound(new { message = ErrorMessages.ProjectNotFound });

        var query = new GetTasksByProjectQuery(projectId);

        var tasks = await _getTasksByProjectUseCase.ExecuteAsync(query);

        var response = tasks.Select(x => new TaskItemResponse
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Status = x.Status,
            ProjectId = x.ProjectId
        });

        return Ok(response);
    }

    private async Task<bool> ProjectBelongsToUserAsync(Guid projectId, Guid userId)
    {
        return await _context.Projects
            .AnyAsync(x => x.Id == projectId && x.UserId == userId);
    }
}
