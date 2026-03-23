using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskFlow.Api.Controllers.Base;

public abstract class AuthenticatedControllerBase : ControllerBase
{
    protected bool TryGetAuthenticatedUserId(out Guid userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out userId);
    }
}
