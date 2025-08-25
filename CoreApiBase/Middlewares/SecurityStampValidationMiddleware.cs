using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Entities;
using System.Security.Claims;

namespace CoreApiBase.Middlewares
{
    public class SecurityStampValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityStampValidationMiddleware> _logger;

        public SecurityStampValidationMiddleware(RequestDelegate next, ILogger<SecurityStampValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServicesBase<User> userService)
        {
            // Only validate authenticated users
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var securityStampClaim = context.User.FindFirst("SecurityStamp")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && !string.IsNullOrEmpty(securityStampClaim))
                {
                    if (int.TryParse(userIdClaim, out var userId))
                    {
                        try
                        {
                            var user = await userService.GetByIdAsync(userId);
                            
                            if (user == null)
                            {
                                _logger.LogWarning("User {UserId} not found in database", userId);
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsync("User not found");
                                return;
                            }

                            // Check if SecurityStamp matches
                            if (user.SecurityStamp != securityStampClaim)
                            {
                                _logger.LogWarning("SecurityStamp mismatch for user {UserId}. Token: {TokenStamp}, DB: {DbStamp}", 
                                    userId, securityStampClaim, user.SecurityStamp);
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsync("Token has been invalidated. Please login again.");
                                return;
                            }

                            _logger.LogDebug("SecurityStamp validation passed for user {UserId}", userId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error validating SecurityStamp for user {UserId}", userId);
                            context.Response.StatusCode = 500;
                            await context.Response.WriteAsync("Internal server error during token validation");
                            return;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Invalid user ID in token: {UserId}", userIdClaim);
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid user ID in token");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
