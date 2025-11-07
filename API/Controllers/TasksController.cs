using API.DTOs;
using API.DTOs.Task;
using API.Services;
using API.Services.TaskManage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private long GetCurrentUserId() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private long GetCurrentFamilyId() => long.Parse(User.FindFirst("FamilyId")?.Value ?? "0");
        private string GetCurrentUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetTasks([FromQuery] string? status = null, [FromQuery] long? assigneeId = null)
        {
            var familyId = GetCurrentFamilyId();
            var tasks = await _taskService.GetTasksAsync(familyId, status, assigneeId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(long id)
        {
            var familyId = GetCurrentFamilyId();
            var task = await _taskService.GetTaskByIdAsync(id, familyId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.DueDate <= DateTime.Now)
                return BadRequest("Due date must be in the future");

            var userId = GetCurrentUserId();
            var familyId = GetCurrentFamilyId();

            var task = await _taskService.CreateTaskAsync(dto, userId, familyId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(long id, CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familyId = GetCurrentFamilyId();
            var task = await _taskService.UpdateTaskAsync(id, dto, familyId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var familyId = GetCurrentFamilyId();
            var success = await _taskService.DeleteTaskAsync(id, familyId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<TaskDto>> UpdateTaskStatus(long id, UpdateTaskStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familyId = GetCurrentFamilyId();
            var task = await _taskService.UpdateTaskStatusAsync(id, dto.Status, familyId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("{id}/assign")]
        public async Task<ActionResult<TaskDto>> AssignTask(long id, [FromBody] List<long> assigneeIds)
        {
            var familyId = GetCurrentFamilyId();
            var task = await _taskService.AssignTaskAsync(id, assigneeIds, familyId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("{id}/verify")]
        public async Task<ActionResult<TaskDto>> VerifyTask(long id)
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "Parent")
                return Forbid("Only parents can verify tasks");

            var userId = GetCurrentUserId();
            var familyId = GetCurrentFamilyId();
            var task = await _taskService.VerifyTaskAsync(id, userId, familyId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet("my-tasks")]
        public async Task<ActionResult<List<TaskDto>>> GetMyTasks([FromQuery] string? status = null)
        {
            var userId = GetCurrentUserId();
            var familyId = GetCurrentFamilyId();
            var tasks = await _taskService.GetTasksAsync(familyId, status, userId);
            return Ok(tasks);
        }
    }
}
