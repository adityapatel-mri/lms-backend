using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using LMS_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
        public async Task<ActionResult<IEnumerable<UserPerformance>>> GetUserPerformances()
        {
            return await _context.UserPerformances.ToListAsync();
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
        public async Task<ActionResult<UserPerformance>> PostUserPerformance([FromBody] UserPerformanceDto userPerformance)
        {
            _context.UserPerformances.Add(userPerformance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserPerformance), new { id = userPerformance.Id }, userPerformance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserPerformance(int id, [FromBody] UserPerformanceDto userPerformanceDto)
        {
            var userPerformance = await _context.UserPerformances.FindAsync(u => u.UserId == id);
            if (userPerformance == null)
            {
                return NotFound();
            }

            userPerformance.UserId = userPerformanceDto.UserId;
            userPerformance.LeadsAssigned = userPerformanceDto.LeadsAssigned;
            userPerformance.LeadsConverted = userPerformanceDto.LeadsConverted;
            userPerformance.LastUpdated = userPerformanceDto.LastUpdated;

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
            return _context.UserPerformances.Any(e => e.Id == id);
        }
    }
}
