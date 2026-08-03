using System.ComponentModel.DataAnnotations;
using System;

namespace TaskManager.Api.Models 
{
    /// <summary>
    /// Represents the status of a task.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Task is pending completion.
        /// </summary>
        Pending,
        
        /// <summary>
        /// Task has been completed.
        /// </summary>
        Completed
    }
    
    /// <summary>
    /// Represents a task item in the system.
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Unique identifier of the task (GUID).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the task.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the task.
        /// </summary>
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        /// <summary>
        /// Date and time when the task was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}