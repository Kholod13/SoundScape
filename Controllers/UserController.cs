using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.DTOs;
using SoundScape.Models;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jwtId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var username = User.Identity?.Name;
            var email = HttpContext.Items["Email"]?.ToString();

            return Ok(new { userId, jwtId, username, email });
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetLoggedInUserData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _dbContext.Users
                .Where(u => u.Id == int.Parse(userId))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(user.AvatarUrl))
            {
                user.AvatarUrl = "/images/default-avatar.png";
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto model, IFormFile? Avatar)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims.");
            }

            var user = await _dbContext.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrEmpty(model.Username))
            {
                user.Username = model.Username;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
            }

            if (model.BirthDay.HasValue)
            {
                user.BirthDay = model.BirthDay.Value;
            }

            if (model.BirthMonth.HasValue)
            {
                user.BirthMonth = model.BirthMonth.Value;
            }

            if (model.BirthYear.HasValue)
            {
                user.BirthYear = model.BirthYear.Value;
            }

            if (!string.IsNullOrEmpty(model.Gender))
            {
                user.Gender = model.Gender;
            }

            if (Avatar != null && Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Avatar.CopyToAsync(fileStream);
                }

                user.AvatarUrl = $"/images/{uniqueFileName}";
            }
            else if (string.IsNullOrEmpty(user.AvatarUrl))
            {
                user.AvatarUrl = "/images/default-avatar.png";
            }

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "User updated successfully", avatarUrl = user.AvatarUrl });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDto model, IFormFile? Avatar)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username, Email, and Password are required.");
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                BirthDay = model.BirthDay,
                BirthMonth = model.BirthMonth,
                BirthYear = model.BirthYear,
                Gender = model.Gender,
                Role = model.Role,
                EmailConfirmed = false 
            };

            if (Avatar != null && Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Avatar.CopyToAsync(fileStream);
                }

                user.AvatarUrl = $"/images/{uniqueFileName}";
            }
            else
            {
                user.AvatarUrl = "/images/default-avatar.png";
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "User created successfully", user });
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Користувача не знайдено.");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Користувача успішно видалено.");
        }
    }
}
