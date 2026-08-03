using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSwaggerGen();

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


