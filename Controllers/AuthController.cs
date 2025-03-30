using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SoundScape.Data;
using SoundScape.DTOs;
using SoundScape.Models;
using SoundScape.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly string _ReactAppUrl;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
            _ReactAppUrl = _configuration["AppSettings:ReactAppUrl"];
        }


       


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {

            if (string.IsNullOrEmpty(model.AvatarUrl))
            {
                model.AvatarUrl = "/images/default-avatar.png";
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                BirthDay = model.BirthDay,
                BirthMonth = model.BirthMonth,
                BirthYear = model.BirthYear,
                Gender = model.Gender,
                EmailConfirmed = false,
                Role = "User"
            
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            /*
            var token = GenerateEmailConfirmationToken(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { token }, Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Email Confirmation", $"Please confirm your email using this link: {confirmationLink}");
            */
            return Ok("Реєстрація успішна. Будь ласка, підтвердіть вашу електронну пошту.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized("Невірний логін або пароль.");
            }

            /* confirmation emeil
            if (!user.EmailConfirmed)
            {
                return Unauthorized("Електронна пошта не підтверджена.");
            }*/

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {

                var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    return BadRequest("Користувача з таким емейлом не знайдено.");
                }

                var token = GeneratePasswordResetToken(user);
                var resetLink = $"{_ReactAppUrl}/forgot-password/NewPassword?token={token}";

                await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Please reset your password by clicking here: <a href='{resetLink}'>link</a>");

                return Ok("Посилання для скидання пароля надіслано на вашу електронну пошту.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto model)
        {
            var user = ValidatePasswordResetToken(model.Token);
            if (user == null)
            {
                return BadRequest("Невірний або прострочений токен.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Пароль успішно змінено.");
        }

        [Authorize]
        [HttpPost("request-email-confirmation")]
        public async Task<IActionResult> RequestEmailConfirmation()
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

            var token = GenerateEmailConfirmationToken(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { token }, Request.Scheme, Request.Host.ToString());

            await _emailService.SendEmailAsync(user.Email, "Email Confirmation", $"Please confirm your email using this link: <a href='{confirmationLink}'>link</a>");

            return Ok("Confirmation email sent. Please check your email.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var user = ValidateEmailConfirmationToken(token);
            if (user == null)
            {
                return BadRequest("Invalid or expired token.");
            }

            user.EmailConfirmed = true;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Email confirmed successfully.");
        }




        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateEmailConfirmationToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User ValidateEmailConfirmationToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

                return _dbContext.Users.Find(userId);
            }
            catch
            {
                return null;
            }
        }

        private string GeneratePasswordResetToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User ValidatePasswordResetToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

                return _dbContext.Users.Find(userId);
            }
            catch
            {
                return null;
            }
        }

    }
}