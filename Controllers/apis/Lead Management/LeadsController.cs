using LMS_Backend.Controllers.apis.Authentication;
using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using LMS_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS_Backend.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly LeadStatusHistoryService _leadStatusHistoryController;

        public LeadController(ApplicationDbContext context, LeadStatusHistoryService leadStatusHistoryController)
        {
            _context = context;
            _leadStatusHistoryController = leadStatusHistoryController;
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

            // Calculate lead counts by status
            var leadCounts = leads.GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Status, g => g.Count);

            // Create the response object
            var response = new
            {
                leads,
                leadCount = new
                {
                    New = leadCounts.ContainsKey("New") ? leadCounts["New"] : 0,
                    Contacted = leadCounts.ContainsKey("Contacted") ? leadCounts["Contacted"] : 0,
                    FollowUp = leadCounts.ContainsKey("FollowUp") ? leadCounts["FollowUp"] : 0,
                    Converted = leadCounts.ContainsKey("Converted") ? leadCounts["Converted"] : 0,
                    Lost = leadCounts.ContainsKey("Lost") ? leadCounts["Lost"] : 0
                }
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Sales")]
        public async Task<IActionResult> UpdateLead(int id, Lead lead)
        {
            if (id != lead.Id) return BadRequest();

            var existingLead = await _context.Leads.FindAsync(id);
            if (existingLead == null) return NotFound();

            var oldStatus = existingLead.Status;
            var newStatus = lead.Status;

            _context.Entry(existingLead).CurrentValues.SetValues(lead);
            await _context.SaveChangesAsync();

            if (oldStatus != newStatus)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int changedBy = int.Parse(userIdClaim.Value);
                await _leadStatusHistoryController.LogStatusChange(id, oldStatus, newStatus, changedBy);
            }

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
