using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Library_API.Exceptions
{
    public class GlobalExceptionsHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionsHandler> _logger;

        public GlobalExceptionsHandler(ILogger<GlobalExceptionsHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception occurred");
            var (statusCode, title) = exception switch
            {
                EntityNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),

                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "Invalid Request"),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            var problem = new ProblemDetails
            {
                Status = statusCode, 
                Title = title,
                Detail = exception?.Message,
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}
