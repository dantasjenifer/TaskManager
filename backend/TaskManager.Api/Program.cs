using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Data;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Interfaces;

var options = new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Production
};

var builder = WebApplication.CreateBuilder(options);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<TaskContext>(optionsDb =>
        optionsDb.UseInMemoryDatabase("TaskManagerDB"));
}
else
{
    builder.Services.AddDbContext<TaskContext>(optionsDb =>
        optionsDb.UseNpgsql(connectionString));
}

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();

builder.Services.AddCors(optionsCors =>
{
    optionsCors.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://taskmanager-frontend-uani.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
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
            swaggerOptions.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not load XML comments: {ex.Message}");
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(uiOptions =>
{
    uiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    uiOptions.RoutePrefix = string.Empty;
});

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();