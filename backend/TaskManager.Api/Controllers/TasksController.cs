using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Api.Services.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        
        /// <summary>
        /// Retrieves the list of all registered tasks.
        /// </summary>
        /// <response code="200">Returns the list of tasks successfully.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Creates a new task in the system.
        /// </summary>
        /// <param name="task">Object containing the data of the task to be created.</param>
        /// <response code="201">Returns the newly created task.</response>
        /// <response code="400">If the provided data is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTasks), new { id = createdTask.Id }, createdTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Completes a specific task by updating its status.
        /// </summary>
        /// <param name="id">Unique identifier (GUID) of the task to be completed.</param>
        /// <response code="204">Task completed successfully (no content returned).</response>
        /// <response code="400">If there is a validation error while completing the task.</response>
        /// <response code="404">Task not found.</response>
        [HttpPatch("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            var result = await _taskService.CompleteTaskAsync(id);

            if (result.IsNotFound)
            {
                return NotFound();
            }

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        /// <summary>
        /// Removes a task from the system by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier (GUID) of the task to be deleted.</param>
        /// <response code="204">Task deleted successfully.</response>
        /// <response code="404">Task not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var success = await _taskService.DeleteTaskAsync(id);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}