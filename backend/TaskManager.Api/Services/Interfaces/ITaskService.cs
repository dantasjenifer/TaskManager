using TaskManager.Api.Models;

namespace TaskManager.Api.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<(bool IsSuccess, bool IsNotFound, string ErrorMessage)> CompleteTaskAsync(Guid id);
        Task<bool> DeleteTaskAsync(Guid id);
    }
}