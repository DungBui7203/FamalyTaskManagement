using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Models;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly FamilyTaskContext _context;

        public ReportsController(FamilyTaskContext context)
        {
            _context = context;
        }

        private long GetCurrentFamilyId() => long.Parse(User.FindFirst("FamilyId")?.Value ?? "0");

        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var familyId = GetCurrentFamilyId();
            var now = DateTime.UtcNow;

            var tasks = await _context.Tasks
                .Where(t => t.FamilyId == familyId && !t.IsArchived.GetValueOrDefault())
                .ToListAsync();

            var summary = new
            {
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status == "Done" || t.Status == "Verified"),
                PendingTasks = tasks.Count(t => t.Status == "Pending"),
                InProgressTasks = tasks.Count(t => t.Status == "InProgress"),
                OverdueTasks = tasks.Count(t => t.DueDate < now && t.Status != "Done" && t.Status != "Verified"),
                TasksByPriority = new
                {
                    High = tasks.Count(t => t.Priority == "High"),
                    Medium = tasks.Count(t => t.Priority == "Medium"),
                    Low = tasks.Count(t => t.Priority == "Low"),
                    Critical = tasks.Count(t => t.Priority == "Critical")
                },
                RecentTasks = tasks
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Status,
                        t.Priority,
                        t.DueDate,
                        t.CreatedAt
                    })
                    .ToList()
            };

            return Ok(summary);
        }
        [HttpGet("member-stats")]
        public async Task<ActionResult<List<TaskMemberStatDto>>> GetMemberTaskStats()
        {
            var familyId = GetCurrentFamilyId();
            var now = DateTime.UtcNow;

            // 1. Lấy tất cả thành viên trong gia đình
            var members = await _context.Users
                .Where(u => u.FamilyId == familyId)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            // 2. Lấy **TaskAssignments** + Task (chỉ task chưa archived)
            //    - IsArchived là bool? → dùng (t.IsArchived == false) hoặc (t.IsArchived == null || t.IsArchived == false)
            var taskAssignments = await _context.TaskAssignments
                .Include(ta => ta.Task)
                .Where(ta => ta.Task.FamilyId == familyId &&
                             (ta.Task.IsArchived == null || ta.Task.IsArchived == false))
                .ToListAsync();

            // 3. Tính thống kê (client-side, vì đã ToListAsync)
            var stats = members.Select(m => new TaskMemberStatDto
            {
                UserId = m.Id,
                FullName = m.FullName ?? "Ẩn danh",
                AssignedCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id),

                PendingCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id &&
                                                        ta.Task.Status == "Pending"),

                InProgressCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id &&
                                                          ta.Task.Status == "InProgress"),

                DoneCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id &&
                                                  (ta.Task.Status == "Done" || ta.Task.Status == "Verified")),

                OnTimeCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id &&
                                                  (ta.Task.Status == "Done" || ta.Task.Status == "Verified") &&
                                                  ta.Task.VerifiedAt != null &&
                                                  ta.Task.VerifiedAt <= ta.Task.DueDate),

                LateCount = taskAssignments.Count(ta => ta.AssigneeId == m.Id &&
                                                  (ta.Task.Status == "Done" || ta.Task.Status == "Verified") &&
                                                  ta.Task.VerifiedAt != null &&
                                                  ta.Task.VerifiedAt > ta.Task.DueDate)
            }).ToList();

            return Ok(stats);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<List<OverdueTaskGroupDto>>> GetOverdueTasks()
        {
            var familyId = GetCurrentFamilyId();
            var now = DateTime.UtcNow;

            var overdueAssignments = await _context.TaskAssignments
                .Include(ta => ta.Task)
                .Include(ta => ta.Assignee)
                .Where(ta => ta.Task.FamilyId == familyId && ta.Task.IsArchived != true
                    && ta.Task.DueDate < now && ta.Task.Status != "Done" && ta.Task.Status != "Verified")
                .ToListAsync();

            var overdueTasks = overdueAssignments.Select(ta => new OverdueTaskDto
            {
                TaskId = ta.TaskId,
                Title = ta.Task.Title,
                DueDate = ta.Task.DueDate,
                AssigneeId = ta.AssigneeId,
                AssigneeName = ta.Assignee.FullName,
                DaysOverdue = (int)(now - ta.Task.DueDate).TotalDays
            }).ToList();

            var grouped = overdueTasks.GroupBy(t => t.AssigneeId)
                .Select(g => new OverdueTaskGroupDto
                {
                    AssigneeId = g.Key,
                    AssigneeName = g.First().AssigneeName,
                    OverdueTasks = g.ToList(),
                    TotalOverdue = g.Count()
                }).ToList();

            return Ok(grouped);
        }
    }
}
