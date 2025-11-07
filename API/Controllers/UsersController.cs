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
    public class UsersController : ControllerBase
    {
        private readonly FamilyTaskContext _context;

        public UsersController(FamilyTaskContext context)
        {
            _context = context;
        }

        private long GetCurrentFamilyId() => long.Parse(User.FindFirst("FamilyId")?.Value ?? "0");

        [HttpGet]
        public async Task<ActionResult<List<object>>> GetFamilyMembers()
        {
            var familyId = GetCurrentFamilyId();
            var users = await _context.Users
                .Where(u => u.FamilyId == familyId && u.IsActive == true)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(long id)
        {
            var familyId = GetCurrentFamilyId();
            var user = await _context.Users
                .Where(u => u.Id == id && u.FamilyId == familyId && u.IsActive == true)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
