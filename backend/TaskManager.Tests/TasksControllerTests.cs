using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using TaskManager.Api.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private TaskContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            
            return new TaskContext(options);
        }
        
        [Fact]
        public async Task CreateTask_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TasksController(context);
            var newTask = new TaskItem { Title = "Test Task", Description = "Test Description" };

            // Act
            var result = await controller.CreateTask(newTask);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal("Test Task", returnedTask.Title);
            Assert.Equal(TaskStatus.Pendente, returnedTask.Status);
        }
    }
}
