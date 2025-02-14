using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_Backend.Models.Entities;
using LMS_Backend.Models;
using BCrypt.Net;
using LMS_Backend.Models.DTOs;

namespace LMS_Backend.Controllers
{

    [ApiController]
    [Route("api/auth")]
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var userRole = user.Role;
            var token = _authService.GenerateJwtToken(user.Id, user.Email, userRole);
            return Ok(new { token });
        }
    }
}
