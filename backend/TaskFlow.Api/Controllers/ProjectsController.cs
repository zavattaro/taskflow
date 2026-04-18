using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Contracts.Projects;
using TaskFlow.Api.Controllers.Base;
using TaskFlow.Application.Projects.CreateProject;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectsController : AuthenticatedControllerBase
{
    private readonly AppDbContext _context;
    private readonly CreateProjectUseCase _createProjectUseCase;

    public ProjectsController(AppDbContext context, CreateProjectUseCase createProjectUseCase)
    {
        _context = context;
        _createProjectUseCase = createProjectUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        var name = request.Name?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Name is required." });

        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        var command = new CreateProjectCommand(userId, name, description);

        var result = await _createProjectUseCase.ExecuteAsync(command);

        var response = new ProjectResponse
        {
            Id = result.ProjectId,
            Name = result.Name,
            Description = result.Description
        };

        return Created($"/api/projects/{result.ProjectId}", response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        var projects = await _context.Projects
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Name)
            .Select(x => new ProjectResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .ToListAsync();

        return Ok(projects);
    }
}
