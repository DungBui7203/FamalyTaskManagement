using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
