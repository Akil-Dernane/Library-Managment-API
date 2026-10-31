using Library_API.Extensions;
using Library_API.Seed;

namespace Library_API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services
        builder.Services.AddLibraryServices(builder.Configuration);

        // Authentication and authorization
        builder.Services.AddLibraryAuthentication(builder.Configuration);
        builder.Services.AddLibraryAuthorization();

        var app = builder.Build();

        // Seed database
        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedAsync(
                scope.ServiceProvider,
                app.Configuration);
        }

        // Configure HTTP pipeline
        app.UseLibraryPipeline();

        await app.RunAsync();
    }
}