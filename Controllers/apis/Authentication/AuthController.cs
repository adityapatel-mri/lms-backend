using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_Backend.Models.Entities;
using LMS_Backend.Models;
using LMS_Backend.Services;
using LMS_Backend.Models.DTOs;
using System.Text.RegularExpressions;

namespace LMS_Backend.Controllers.apis.Authentication
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthController(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!IsValidEmail(model.Email) || !IsValidPassword(model.Password))
            {
                return BadRequest(new { message = "Invalid email or password." });
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = hashedPassword
            };

            // Save user to the database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully registered. Please Login :)" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!IsValidEmail(model.Email) || !IsValidPassword(model.Password))
            {
                return BadRequest(new { message = "Invalid email or password." });
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _authService.GenerateJwtToken(user.Id, user.Email, user.Role ?? "Sales");

        
            // Store the token in the session
            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("Role", user.Role ?? "Sales");

            return Ok(new { message = "Login successful.", authToken = token});
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
           
            HttpContext.Session.Clear();

            return Ok(new { message = "Logout successful" });
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }
    }
}
