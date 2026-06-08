using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts;
using TaskFlow.Api.Contracts.Projects;
using TaskFlow.Api.Controllers.Base;
using TaskFlow.Api.Errors;
using TaskFlow.Application.Projects.CreateProject;
using TaskFlow.Application.Projects.GetAllProjects;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectsController : AuthenticatedControllerBase
{
    private readonly CreateProjectUseCase _createProjectUseCase;
    private readonly GetAllProjectsUseCase _getAllProjectsUseCase;

    public ProjectsController(
        CreateProjectUseCase createProjectUseCase,
        GetAllProjectsUseCase getAllProjectsUseCase)
    {
        _createProjectUseCase = createProjectUseCase;
        _getAllProjectsUseCase = getAllProjectsUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request, CancellationToken ct)
    {
        var name = request.Name?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new ApiErrorResponse(ErrorMessages.NameRequired));

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidUserContext));

        var command = new CreateProjectCommand(userId, name, description);
        var result = await _createProjectUseCase.ExecuteAsync(command, ct);

        var response = new ProjectResponse(result.ProjectId, result.Name, result.Description);
        return Created($"/api/projects/{result.ProjectId}", response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new ApiErrorResponse(ErrorMessages.InvalidUserContext));

        var query = new GetAllProjectsQuery(userId);
        var projects = await _getAllProjectsUseCase.ExecuteAsync(query, ct);

        var response = projects.Select(x => new ProjectResponse(x.Id, x.Name, x.Description));
        return Ok(response);
    }
}
