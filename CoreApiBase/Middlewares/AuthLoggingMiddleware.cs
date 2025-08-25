namespace CoreApiBase.Middlewares
{
    public class AuthLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthLoggingMiddleware> _logger;

        public AuthLoggingMiddleware(RequestDelegate next, ILogger<AuthLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
                _logger.LogInformation("Authenticated request from User: {Username} (ID: {UserId}) to {Method} {Path}",
                    username, userId, context.Request.Method, context.Request.Path);
            }

            await _next(context);
        }
    }
}
