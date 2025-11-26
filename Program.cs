using MarkItDoneApi.Src.Infra.Data;
using MarkItDoneApi.Src.V1.User.Repository;
using MarkItDoneApi.Src.V1.User.Service;
using MarkItDoneApi.Src.V1.Session.Repository;
using MarkItDoneApi.Src.V1.Session.Service;
using MarkItDoneApi.Src.V1.Core.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json;
using MarkItDoneApi.Src.V1.Email;
using MarkItDoneApi.Src.V1.Migration.Service;

var builder = WebApplication.CreateBuilder(args);

// Adds native support to OpenAPI/Swagger
builder.Services.AddOpenApi();

// Adds controllers support
builder.Services.AddControllers();

// Configure JSON options for camelCase naming
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// Configure Entity Framework for migrations only
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adds services support
builder.Services.AddScoped<ConnectionFactory>();
builder.Services.AddScoped<DatabaseStatusChecker>();

// Register User layer dependencies
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

// Register Session layer dependencies
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<SessionService>();

// Register Email service
builder.Services.AddScoped<EmailService>();

// Register Migration service
builder.Services.AddScoped<MigrationService>();

var app = builder.Build();

// Register exception handling middleware (FIRST)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configures pipeline requests
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Adds Scalar UI using the official package
    app.MapScalarApiReference();
    
    // Redirecting to easier access
    app.MapGet("/docs", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Simple root route
app.MapGet("/", () => "MarkItDone API OK!");

app.Run();
