
using Library_API.Data;
using Library_API.Exceptions;
using Library_API.Middlewares;
using Library_API.Models;
using Library_API.Repositories;
using Library_API.Repositories.Interfaces;
using Library_API.Seed;
using Library_API.Services;
using Library_API.Services.Admin;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation;
using Library_API.Services.Validation.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Library_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Register API Settings
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
            builder.Services.Configure<LibrarySettings>(
                builder.Configuration.GetSection("LibrarySettings"));

            //Jwt
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<ITokenService, TokenService>();

            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

            builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    RoleClaimType = "role"
                };
            });

            builder.Services.AddAuthorization();

            //services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped< ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped< IAuthService, AuthService>();

            builder.Services.AddScoped< IClaimValidationService, ClaimValidationService>();

            builder.Services.AddScoped< IAdminUserService, AdminUserService>();
            builder.Services.AddScoped< IAdminRolesService, AdminRolesService >();


            builder.Services.AddScoped< IMemberService, MemberService>();
            builder.Services.AddScoped< IAuthorService, AuthorService>();
            builder.Services.AddScoped< IBookService, BookService>();
            builder.Services.AddScoped< ICategoryService, CategoryService>();
            builder.Services.AddScoped< ILoanService, LoanService>();



            //Repositories
            builder.Services.AddScoped< IMemberRepository, MemberRepository>();
            builder.Services.AddScoped< IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped< IBookRepository, BookRepository>();
            builder.Services.AddScoped< ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped< ILoanRepository, LoanRepository>();

            //Database Configuration
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

            builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                // passwords
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // users
                options.User.RequireUniqueEmail = true;

                // lockout
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();


            // Policies
            var authorizationBuilder = builder.Services.AddAuthorizationBuilder()
                .AddPolicy(Policies.StaffOnly, policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireRole(Roles.Admin, Roles.Librarian))
                .SetFallbackPolicy(
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build());

            foreach (var permission in AppPermissions.All)
            {
                authorizationBuilder.AddPolicy(permission, policy =>
                    policy.RequireClaim(AppClaims.Permission, permission));
            }

            //Global Exception Handler
            builder.Services.AddExceptionHandler<GlobalExceptionsHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
