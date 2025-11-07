using API.DTOs.User;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpPost]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familyId = GetCurrentFamilyId();

            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.IsActive == true);

            if (existingUser)
                return BadRequest("Email already exists");

            var user = new User
            {
                FamilyId = familyId,
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<UserDto>> UpdateUser(long id, UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var familyId = GetCurrentFamilyId();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.FamilyId == familyId && u.IsActive == true);

            if (user == null)
                return NotFound();

            user.FullName = dto.FullName;
            user.Role = dto.Role;

            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            var familyId = GetCurrentFamilyId();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.FamilyId == familyId && u.IsActive == true);

            if (user == null)
                return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
