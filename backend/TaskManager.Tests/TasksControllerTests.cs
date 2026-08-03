using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Controllers;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using Xunit;
using TaskStatus = TaskManager.Api.Models.TaskStatus;

namespace TaskManager.Tests
{
    public class TasksControllerTests
    {
        private TaskContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new TaskContext(options);
        }

        // --- GET TASKS ---
        [Fact]
        public async Task GetTasks_ReturnsAllTasks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Tasks.Add(new TaskItem { Title = "Task 1" });
            context.Tasks.Add(new TaskItem { Title = "Task 2" });
            await context.SaveChangesAsync();
            var controller = new TasksController(context);

            // Act
            var result = await controller.GetTasks();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskItem>>>(result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(actionResult.Value);
            Assert.Equal(2, tasks.Count());
        }

        // --- CREATE TASK ---
        [Fact]
        public async Task CreateTask_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TasksController(context);
            var newTask = new TaskItem { Title = "Study TDD", Description = "Review unit tests" };

            // Act
            var result = await controller.CreateTask(newTask);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal("Study TDD", returnedTask.Title);
            Assert.Equal(TaskStatus.Pending, returnedTask.Status);
        }

        [Fact]
        public async Task CreateTask_WithoutTitle_ReturnsBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TasksController(context);
            var invalidTask = new TaskItem { Title = "", Description = "No title test" };

            // Act
            var result = await controller.CreateTask(invalidTask);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // --- COMPLETE TASK (PATCH) ---
        [Fact]
        public async Task CompleteTask_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Task to complete" };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var controller = new TasksController(context);

            // Act
            var result = await controller.CompleteTask(task.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedTask = await context.Tasks.FindAsync(task.Id);
            Assert.Equal(TaskStatus.Completed, updatedTask.Status);
        }

        [Fact]
        public async Task CompleteTask_AlreadyCompleted_ReturnsBadRequest()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Already completed task", Status = TaskStatus.Completed };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var controller = new TasksController(context);

            // Act
            var result = await controller.CompleteTask(task.Id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Task is already completed.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompleteTask_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.CompleteTask(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // --- DELETE TASK ---
        [Fact]
        public async Task DeleteTask_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Task to delete" };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(task.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Tasks);
        }

        [Fact]
        public async Task DeleteTask_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new TasksController(context);

            // Act
            var result = await controller.DeleteTask(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}