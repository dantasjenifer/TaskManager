namespace TaskManager.Api.Models 
{
    public enum TaskStatus
    {
        Pending,
        Completed
    }
    
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

