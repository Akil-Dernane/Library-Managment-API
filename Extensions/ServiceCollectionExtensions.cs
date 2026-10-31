using Library_API.Auditing;
using Library_API.Authorization;
using Library_API.Data;
using Library_API.Exceptions;
using Library_API.Models;
using Library_API.Repositories;
using Library_API.Repositories.Interfaces;
using Library_API.Services;
using Library_API.Services.Admin;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Background;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation;
using Library_API.Services.Validation.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

namespace Library_API.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Settings
        services.Configure<ApiSettings>(
            configuration.GetSection("ApiSettings"));

        services.Configure<LibrarySettings>(
            configuration.GetSection("LibrarySettings"));

        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

        // Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);
        })
        .AddRoles<IdentityRole<int>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // HTTP context
        services.AddHttpContextAccessor();

        // Application services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClaimValidationService, ClaimValidationService>();

        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAdminRolesService, AdminRolesService>();

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ILoanService, LoanService>();

        // Repositories
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();

        // Authorization handlers
        services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();

        // Background services
        services.AddHostedService<RefreshTokenCleanupService>();

        // Rate limiting
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(Policies.Auth, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey:
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        // Exception handling
        services.AddExceptionHandler<GlobalExceptionsHandler>();
        services.AddProblemDetails();

        // API
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}