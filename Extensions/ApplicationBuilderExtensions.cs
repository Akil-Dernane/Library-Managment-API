using Library_API.Middlewares;

namespace Library_API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseLibraryPipeline(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseRateLimiter();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}