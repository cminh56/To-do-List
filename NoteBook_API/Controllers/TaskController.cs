using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteBook_API.DTO.TaskDTO;
using NoteBook_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoteBook_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly PRN231_NoteContext _context;

        public TaskController(PRN231_NoteContext context)
        {
            _context = context;
        }

        // GET: api/Task
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetAllTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.User) // Include user data if necessary
                .ToListAsync();

            // Map Task to TaskDTO
            var taskDTOs = tasks.Select(t => new TaskDTO
            {
                TaskId = t.TaskId,
                UserId = (int)t.UserId,
                Title = t.Title,
                Description = t.Description,
                DueDate = (DateTime)t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                UpdatedAt = t.UpdatedAt,
          
            }).ToList();

            return Ok(taskDTOs);
        }


        // GET: api/Task/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasksByUserId(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Include(t => t.User) // Include user data if needed
                .ToListAsync();

      
            var taskDTOs = tasks.Select(t => new TaskDTO
            {
                TaskId = t.TaskId,
                UserId = (int)t.UserId,
                Title = t.Title,
                Description = t.Description,
                DueDate = (DateTime)t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                UpdatedAt = t.UpdatedAt,
              
            }).ToList();

            return Ok(taskDTOs);
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskDTO>> GetTaskById(int taskId)
        {
            var task = await _context.Tasks
                .Where(t => t.TaskId == taskId)
                .Select(t => new TaskDTO
                {
                    TaskId = t.TaskId,
                    UserId = (int)t.UserId,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = (DateTime)t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            return Ok(task);
        }


        // POST: api/Task
        [HttpPost]
        public async Task<ActionResult<TaskDTO>> AddTask(TaskDTO taskDTO)
        {
            // Validate the incoming data (ensure description is not null or empty)
            if (taskDTO == null || string.IsNullOrEmpty(taskDTO.Title))
            {
                return BadRequest("Invalid task data. Title is required.");
            }

            if (taskDTO.Description != null && taskDTO.Description.Length > 1000) // You can validate the length
            {
                return BadRequest("Description is too long.");
            }

            // Create a new Task entity from the DTO
            var task = new Models.Task
            {
                UserId = taskDTO.UserId, // Assign the UserId
                Title = taskDTO.Title,
                Description = taskDTO.Description, // Vietnamese characters should be preserved here
                DueDate = taskDTO.DueDate,
                Priority = taskDTO.Priority,
                Status = taskDTO.Status,
                UpdatedAt = DateTime.UtcNow.AddHours(7) // Set the updated time to now
            };

            // Add the task to the context
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Return the created task as a DTO
            var createdTaskDTO = new TaskDTO
            {
                TaskId = task.TaskId,
                UserId = task.UserId,
                Title = task.Title,
                Description = task.Description, // This will retain the Vietnamese characters
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                UpdatedAt = task.UpdatedAt
            };

            // Return a successful response with the created task
            return CreatedAtAction(nameof(GetTasksByUserId), new { userId = task.UserId }, createdTaskDTO);
        }


        // PUT: api/Task/{taskId}
        [HttpPut("{taskId}")]
        public async Task<IActionResult> EditTask(int taskId, TaskDTO taskDTO)
        {
      

            // Find the existing task by ID
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            // Update the task fields
            task.Title = taskDTO.Title;
            task.Description = taskDTO.Description;
            task.DueDate = taskDTO.DueDate;
            task.Priority = taskDTO.Priority;
            task.Status = taskDTO.Status;
            task.UpdatedAt = DateTime.UtcNow.AddHours(7); // Update the last modified date

            // Save the changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(taskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content on successful update
        }

        private bool TaskExists(int taskId)
        {
            return _context.Tasks.Any(e => e.TaskId == taskId);
        }

        // DELETE: api/Task/{taskId}
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            // Find the existing task by ID
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound($"Task with ID {taskId} not found.");
            }

            // Remove the task from the database
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content on successful deletion
        }

    }
}
