using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using LMS_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace LMS_Backend.Controllers.APIs
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPerformanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserPerformanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetUserPerformances()
        {
            // Extract user information from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || userRoleClaim == null)
            {
                return Unauthorized(new { message = "Invalid user token." });
            }

            int userId = int.Parse(userIdClaim.Value);
            string userRole = userRoleClaim.Value;

            IQueryable<UserPerformance> query = _context.UserPerformances;
            if (userRole == "Manager")
            {
                // Get IDs of Sales Reps under the manager
                var salesReps = await _context.Users
                    .Where(u => u.Role == "Sales" && u.ReportsTo == userId) // Sales Reps reporting to this Manager
                    .Select(u => u.Id)
                    .ToListAsync();

                query = query.Where(up => salesReps.Contains(up.UserId)); // Filter UP of those Sales Reps
            }
            else if (userRole == "Admin")
            {
                var managers = await _context.Users
                    .Where(u => u.Role == "Manager" && u.ReportsTo == userId) // Manager reporting to this Admin
                    .Select(u => u.Id)
                    .ToListAsync();

                query = query.Where(up => managers.Contains(up.UserId));
            }
            else
            {
                return Forbid(); // Unknown role
            }

            var userPerformances = await query.ToListAsync();
            return Ok(userPerformances);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserPerformance>> GetUserPerformance(int id)
        {
            var userPerformance = await _context.UserPerformances
                .FirstOrDefaultAsync(up => up.UserId == id);

            if (userPerformance == null)
            {
                return NotFound();
            }

            return userPerformance;
        }

        [HttpPost]
        public async Task<ActionResult<UserPerformance>> PostUserPerformance([FromBody] UserPerformanceDto userPerformanceDto)
        {
            var userPerformance = new UserPerformance
            {
                UserId = userPerformanceDto.UserId,
                LeadsAssigned = userPerformanceDto.LeadsAssigned,
                LeadsConverted = userPerformanceDto.LeadsConverted,
            };

            _context.UserPerformances.Add(userPerformance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserPerformance), new { id = userPerformance.Id }, userPerformance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPerformance(int id, [FromBody] UserPerformanceDto userPerformanceDto)
        {
            var userPerformance = await _context.UserPerformances.FirstOrDefaultAsync(u => u.UserId == id);
            if (userPerformance == null)
            {
                return NotFound();
            }

            userPerformance.UserId = userPerformanceDto.UserId;
            userPerformance.LeadsAssigned = userPerformanceDto.LeadsAssigned;
            userPerformance.LeadsConverted = userPerformanceDto.LeadsConverted;

            _context.Entry(userPerformance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserPerformanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPerformance(int id)
        {
            var userPerformance = await _context.UserPerformances.FindAsync(id);
            if (userPerformance == null)
            {
                return NotFound();
            }

            _context.UserPerformances.Remove(userPerformance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserPerformanceExists(int id)
        {
            return _context.UserPerformances.Any(e => e.UserId == id);
        }
    }
}