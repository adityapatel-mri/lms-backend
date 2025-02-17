using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeads()
        {
            // Extract user information from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || userRoleClaim == null)
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            int userId = int.Parse(userIdClaim.Value);
            string userRole = userRoleClaim.Value;

            IQueryable<Lead> query = _context.Leads;

            if (userRole == "Sales")
            {
                // Sales Rep can only see their own assigned leads
                query = query.Where(l => l.AssignedTo == userId);
            }
            else if (userRole == "Manager")
            {
                // Get IDs of Sales Reps under the manager
                var salesReps = await _context.Users
                    .Where(u => u.Role == "Sales" && u.ReportsTo == userId) // Sales Reps reporting to this Manager
                    .Select(u => u.Id)
                    .ToListAsync();

                query = query.Where(l => salesReps.Contains(l.AssignedTo ?? 0)); // Filter leads assigned to those Sales Reps
            }
            else if (userRole == "Admin")
            {
                // Admin gets all leads (no filter needed)
            }
            else
            {
                return Forbid(); // Unknown role
            }

            var leads = await query.ToListAsync();
            return Ok(leads);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin,Manager")]
        public async Task<ActionResult<Lead>> GetLead(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null) return NotFound();
            return lead;
        }

        [HttpPost]
        public async Task<ActionResult<Lead>> CreateLead(Lead lead)
        {
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLead), new { id = lead.Id }, lead);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLead(int id, Lead lead)
        {
            if (id != lead.Id) return BadRequest();
            _context.Entry(lead).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null) return NotFound();
            _context.Leads.Remove(lead);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
