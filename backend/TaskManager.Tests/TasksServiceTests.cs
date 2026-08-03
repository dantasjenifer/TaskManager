using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Interfaces;
using Xunit;
using TaskStatus = TaskManager.Api.Models.TaskStatus;

namespace TaskManager.Tests
{
    public class TasksServiceTests
    {
        private TaskContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new TaskContext(options);
        }

        // --- GET ALL TASKS ---
        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Tasks.Add(new TaskItem { Title = "Task 1" });
            context.Tasks.Add(new TaskItem { Title = "Task 2" });
            await context.SaveChangesAsync();
            
            var service = new TaskService(context);

            // Act
            var result = await service.GetAllTasksAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        // --- CREATE TASK ---
        [Fact]
        public async Task CreateTaskAsync_WithValidData_ReturnsTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TaskService(context);
            var newTask = new TaskItem { Title = "Study Services", Description = "Refactor logic" };

            // Act
            var result = await service.CreateTaskAsync(newTask);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Study Services", result.Title);
            Assert.Equal(TaskStatus.Pending, result.Status);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task CreateTaskAsync_WithoutTitle_ThrowsArgumentException()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TaskService(context);
            var invalidTask = new TaskItem { Title = "", Description = "No title test" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTaskAsync(invalidTask));
            Assert.Contains("Title is required", exception.Message);
        }

        // --- COMPLETE TASK ---
        [Fact]
        public async Task CompleteTaskAsync_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Task to complete" };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            
            var service = new TaskService(context);

            // Act
            var result = await service.CompleteTaskAsync(task.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsNotFound);
            Assert.Empty(result.ErrorMessage);

            var updatedTask = await context.Tasks.FindAsync(task.Id);
            Assert.Equal(TaskStatus.Completed, updatedTask.Status);
        }

        [Fact]
        public async Task CompleteTaskAsync_AlreadyCompleted_ReturnsError()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Already completed task", Status = TaskStatus.Completed };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            
            var service = new TaskService(context);

            // Act
            var result = await service.CompleteTaskAsync(task.Id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(result.IsNotFound);
            Assert.Equal("Task is already completed", result.ErrorMessage);
        }

        [Fact]
        public async Task CompleteTaskAsync_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TaskService(context);

            // Act
            var result = await service.CompleteTaskAsync(Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsNotFound);
            Assert.Empty(result.ErrorMessage);
        }

        // --- DELETE TASK ---
        [Fact]
        public async Task DeleteTaskAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var task = new TaskItem { Title = "Task to delete" };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
            
            var service = new TaskService(context);

            // Act
            var result = await service.DeleteTaskAsync(task.Id);

            // Assert
            Assert.True(result);
            Assert.Empty(context.Tasks);
        }

        [Fact]
        public async Task DeleteTaskAsync_NonExistentId_ReturnsFalse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TaskService(context);

            // Act
            var result = await service.DeleteTaskAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }
    }
}