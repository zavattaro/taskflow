using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Contracts.Projects;
using TaskFlow.Api.Controllers.Base;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectsController : AuthenticatedControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
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

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            UserId = userId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var response = new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description
        };

        return Created($"/api/projects/{project.Id}", response);
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
