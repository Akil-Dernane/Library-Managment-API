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
            var (statusCode, title) = exception switch
            {
                EntityNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),

                ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "Invalid Request"),

                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            var logLevel = statusCode switch
            {
                StatusCodes.Status500InternalServerError => LogLevel.Error,
                StatusCodes.Status409Conflict => LogLevel.Warning,
                _ => LogLevel.Information
            };

            _logger.Log(logLevel, exception, "Request failed with status code {StatusCode}", statusCode);

            var problem = new ProblemDetails
            {
                Status = statusCode, 
                Title = title,  
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}
