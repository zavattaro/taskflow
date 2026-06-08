using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts;
using TaskFlow.Api.Contracts.Tasks;
using TaskFlow.Api.Controllers.Base;
using TaskFlow.Api.Errors;
using TaskFlow.Application.Tasks.CreateTaskItem;
using TaskFlow.Application.Tasks.GetTasksByProject;
using TaskFlow.Application.Tasks.UpdateTaskStatus;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController : AuthenticatedControllerBase
{
    private readonly CreateTaskItemUseCase _createTaskItemUseCase;
    private readonly UpdateTaskStatusUseCase _updateTaskStatusUseCase;
    private readonly GetTasksByProjectUseCase _getTasksByProjectUseCase;

    public TasksController(
        CreateTaskItemUseCase createTaskItemUseCase,
        UpdateTaskStatusUseCase updateTaskStatusUseCase,
        GetTasksByProjectUseCase getTasksByProjectUseCase)
    {
        _createTaskItemUseCase = createTaskItemUseCase;
        _updateTaskStatusUseCase = updateTaskStatusUseCase;
        _getTasksByProjectUseCase = getTasksByProjectUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskItemRequest request, CancellationToken ct)
    {
        var title = request.Title?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new ApiErrorResponse(ErrorMessages.TitleRequired));

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidUserContext));

        var command = new CreateTaskItemCommand(projectId, userId, title, description);
        var result = await _createTaskItemUseCase.ExecuteAsync(command, ct);

        var response = new TaskItemResponse(
            result.TaskItemId, result.Title, result.Description,
            result.Status, result.ProjectId
        );

        return Created($"/api/projects/{projectId}/tasks/{result.TaskItemId}", response);
    }

    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid projectId, Guid taskId, UpdateTaskItemStatusRequest request, CancellationToken ct)
    {
        var statusValue = request.Status?.Trim();

        if (string.IsNullOrWhiteSpace(statusValue))
            return BadRequest(new ApiErrorResponse(ErrorMessages.StatusRequired));

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidUserContext));

        var command = new UpdateTaskStatusCommand(projectId, userId, taskId, statusValue);
        var result = await _updateTaskStatusUseCase.ExecuteAsync(command, ct);

        var response = new TaskItemResponse(
            result.TaskItemId, result.Title, result.Description,
            result.Status, result.ProjectId
        );

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken ct)
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidUserContext));

        var query = new GetTasksByProjectQuery(projectId, userId);
        var tasks = await _getTasksByProjectUseCase.ExecuteAsync(query, ct);

        var response = tasks.Select(x => new TaskItemResponse(
            x.Id, x.Title, x.Description, x.Status, x.ProjectId
        ));

        return Ok(response);
    }
}
