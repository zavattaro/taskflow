using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Contracts.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskItemRequest request)
    {
        var title = request.Title?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest(new { message = "Title is required." });

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        var projectExists = await _context.Projects
            .AnyAsync(x => x.Id == projectId && x.UserId == userId);

        if (!projectExists)
            return NotFound(new { message = "Project not found." });

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            Status = TaskItemsStatus.Todo,
            ProjectId = projectId
        };

        _context.Tasks.Add(taskItem);
        await _context.SaveChangesAsync();

        var response = new TaskItemResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            ProjectId = taskItem.ProjectId
        };

        return Created($"/api/projects/{projectId}/tasks/{taskItem.Id}", response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid projectId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid user context." });

        var projectExists = await _context.Projects
            .AnyAsync(x => x.Id == projectId && x.UserId == userId);

        if (!projectExists)
            return NotFound(new { message = "Project not found." });

        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.Title)
            .Select(x => new TaskItemResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status.ToString(),
                ProjectId = x.ProjectId
            })
            .ToListAsync();

        return Ok(tasks);
    }
}
