
using Library_API.Data;
using Library_API.Exceptions;
using Library_API.Models;
using Library_API.Repositories;
using Library_API.Repositories.Interfaces;
using Library_API.Services;
using Library_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Register API Settings
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            //services
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

            //Database connection
            builder.Services.AddDbContext<AppDbContext>( options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

            //Exception Handler
            builder.Services.AddExceptionHandler<GlobalExceptionsHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
