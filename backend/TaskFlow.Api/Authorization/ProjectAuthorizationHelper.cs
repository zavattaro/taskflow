using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Authorization;

public static class ProjectAuthorizationHelper
{
    public static async Task<bool> UserOwnsProjectAsync(AppDbContext context, Guid projectId, Guid userId)
    {
        return await context.Projects
            .AnyAsync(p => p.Id == projectId && p.UserId == userId);
    }
}