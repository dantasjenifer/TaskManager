using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Data;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<TaskContext>(options =>
        options.UseInMemoryDatabase("TaskManagerDB"));
}
else
{
    builder.Services.AddDbContext<TaskContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://task-manager-ui.onrender.com",
                "https://task-manager-ui.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "Task management API developed for the project.",
        Contact = new OpenApiContact
        {
            Name = "Jenifer"
        }
    });

    try
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
    catch
    {
    }
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    options.RoutePrefix = string.Empty;
});

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();