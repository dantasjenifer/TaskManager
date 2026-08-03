using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services.Interfaces;

namespace TaskManager.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskContext _taskContext;

        public TaskService(TaskContext context)
        {
            _taskContext = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskContext.Tasks.ToListAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                throw new ArgumentException("Title is required", nameof(task));
            }

            _taskContext.Tasks.Add(task);
            await _taskContext.SaveChangesAsync();

            return task;
        }

        public async Task<(bool IsSuccess, bool IsNotFound, string ErrorMessage)> CompleteTaskAsync(Guid Id)
        {
            var task = await _taskContext.Tasks.FindAsync(Id);

            if (task == null)
            {
                return (false, true, string.Empty);
            }

            if (task.Status == Models.TaskStatus.Completed)
            {
                return (false, false, "Task is already completed");
            }

            task.Status = Models.TaskStatus.Completed;
            await _taskContext.SaveChangesAsync();

            return (true, false, string.Empty);
        }

        public async Task<bool> DeleteTaskAsync(Guid Id)
        {
            var task = await _taskContext.Tasks.FindAsync(Id);

            if (task == null)
            {
                return false;
            }

            _taskContext.Tasks.Remove(task);
            await _taskContext.SaveChangesAsync();

            return true;
        }
    }
}