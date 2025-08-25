using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Entities;
using CoreDomainBase.Enums;
using System.Security.Claims;

namespace CoreApiBase.Middlewares
{
    public class DatabaseRoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseRoleAuthorizationMiddleware> _logger;

        public DatabaseRoleAuthorizationMiddleware(RequestDelegate next, ILogger<DatabaseRoleAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServicesBase<User> userService)
        {
            // Only check for authenticated users
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    try
                    {
                        // Get user from database
                        var user = await userService.GetByIdAsync(userId);
                        
                        if (user == null)
                        {
                            _logger.LogWarning("User {UserId} not found in database", userId);
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("User not found");
                            return;
                        }

                        // Create new identity with current role from database
                        var identity = new ClaimsIdentity(context.User.Identity.AuthenticationType);
                        
                        // Copy existing claims except role
                        foreach (var claim in context.User.Claims.Where(c => c.Type != ClaimTypes.Role))
                        {
                            identity.AddClaim(claim);
                        }
                        
                        // Add current role from database
                        identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
                        
                        // Replace the user principal with updated role
                        context.User = new ClaimsPrincipal(identity);
                        
                        _logger.LogInformation("✅ Updated user {UserId} role to {Role} from database", userId, user.Role);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error fetching user {UserId} from database", userId);
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Internal server error during role validation");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
