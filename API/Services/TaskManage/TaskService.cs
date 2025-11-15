using API.DTOs;
using API.DTOs.Task;
using API.Models;
using API.Services;
using API.Services.TaskManage;
using Microsoft.EntityFrameworkCore;
using TaskModel = API.Models.Task;

namespace API.Services.TaskManage
{
    public class TaskService : ITaskService
    {
        private readonly FamilyTaskContext _context;

        public TaskService(FamilyTaskContext context)
        {
            _context = context;
        }
        public async Task<List<TaskDto>> GetTasksAsync(long familyId, string? status = null, long? assigneeId = null)
        {
            var query = _context.Tasks
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.VerifiedByNavigation)
                .Include(t => t.TaskAssignments)
                    .ThenInclude(ta => ta.Assignee)
                .Where(t => t.FamilyId == familyId && (t.IsArchived == null || t.IsArchived == false));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            if (assigneeId.HasValue)
                query = query.Where(t => t.TaskAssignments.Any(ta => ta.AssigneeId == assigneeId));

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(long id, long familyId)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.VerifiedByNavigation)
                .Include(t => t.TaskAssignments)
                    .ThenInclude(ta => ta.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id && t.FamilyId == familyId);

            return task != null ? MapToDto(task) : null;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, long createdBy, long familyId)
        {
            var task = new Models.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Status = "Pending",
                CreatedBy = createdBy,
                FamilyId = familyId,
                IsArchived = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Add assignments
            if (dto.AssigneeIds.Any())
            {
                var assignments = dto.AssigneeIds.Select(assigneeId => new TaskAssignment
                {
                    TaskId = task.Id,
                    AssigneeId = assigneeId,
                    Status = "Pending", // Thêm dòng này
                    AssignedAt = DateTime.UtcNow
                }).ToList();

                _context.TaskAssignments.AddRange(assignments);
                await _context.SaveChangesAsync();
            }

            return await GetTaskByIdAsync(task.Id, familyId) ?? throw new Exception("Task not found after creation");
        }

        public async Task<TaskDto?> UpdateTaskAsync(long id, CreateTaskDto dto, long familyId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.FamilyId == familyId);
            if (task == null) return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Category = dto.Category;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetTaskByIdAsync(id, familyId);
        }

        public async Task<bool> DeleteTaskAsync(long id, long familyId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.FamilyId == familyId);
            if (task == null) return false;

            task.IsArchived = true;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskDto?> UpdateTaskStatusAsync(long taskId, string status, long familyId, long? userId = null)
        {
            if (userId.HasValue)
            {
                // Member cập nhật assignment của mình
                var assignment = await _context.TaskAssignments
                    .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.AssigneeId == userId.Value);

                if (assignment == null) return null;

                assignment.Status = status;
                if (status == "Done")
                    assignment.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await UpdateTaskOverallStatus(taskId);
            }
            else
            {
                // Parent cập nhật toàn bộ task
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.FamilyId == familyId);
                if (task == null) return null;

                task.Status = status;
                task.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return await GetTaskByIdAsync(taskId, familyId);
        }

        private async System.Threading.Tasks.Task UpdateTaskOverallStatus(long taskId)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskAssignments)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return;

            var assignments = task.TaskAssignments.ToList();
            if (!assignments.Any()) return;

            if (assignments.All(a => a.Status == "Done"))
                task.Status = "Done";
            else if (assignments.Any(a => a.Status == "InProgress"))
                task.Status = "InProgress";
            else if (assignments.Any(a => a.Status == "Done"))
                task.Status = "InProgress";
            else
                task.Status = "Pending";

            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<TaskDto?> AssignTaskAsync(long id, List<long> assigneeIds, long familyId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.FamilyId == familyId);
            if (task == null) return null;

            // Remove existing assignments
            var existingAssignments = await _context.TaskAssignments.Where(ta => ta.TaskId == id).ToListAsync();
            _context.TaskAssignments.RemoveRange(existingAssignments);

            // Add new assignments
            var newAssignments = assigneeIds.Select(assigneeId => new TaskAssignment
            {
                TaskId = id,
                AssigneeId = assigneeId,
                Status = "Pending", // Thêm dòng này
                AssignedAt = DateTime.UtcNow
            }).ToList();

            _context.TaskAssignments.AddRange(newAssignments);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(id, familyId);
        }

        public async Task<TaskDto?> VerifyTaskAsync(long id, long verifiedBy, long familyId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.FamilyId == familyId);
            if (task == null) return null;

            task.Status = "Verified";
            task.VerifiedBy = verifiedBy;
            task.VerifiedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(id, familyId);
        }

        private static TaskDto MapToDto(Models.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Category = task.Category,
                Priority = task.Priority ?? "Medium",
                DueDate = task.DueDate,
                Status = task.Status ?? "Pending",
                CreatedBy = task.CreatedBy,
                CreatedByName = task.CreatedByNavigation.FullName,
                VerifiedBy = task.VerifiedBy,
                VerifiedByName = task.VerifiedByNavigation?.FullName,
                VerifiedAt = task.VerifiedAt,
                IsArchived = task.IsArchived ?? false,
                CreatedAt = task.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = task.UpdatedAt ?? DateTime.MinValue,
                AssigneeNames = task.TaskAssignments.Select(ta => ta.Assignee.FullName).ToList(),
                AssigneeIds = task.TaskAssignments.Select(ta => ta.AssigneeId).ToList(),
                Assignments = task.TaskAssignments.Select(ta => new TaskAssignmentDto // Thêm phần này
                {
                    AssigneeId = ta.AssigneeId,
                    AssigneeName = ta.Assignee.FullName,
                    Status = ta.Status,
                    AssignedAt = ta.AssignedAt,
                    CompletedAt = ta.CompletedAt
                }).ToList()
            };
        }

    }
}
