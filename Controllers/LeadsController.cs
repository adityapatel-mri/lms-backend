using LMS_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeads()
        {
            return await _context.Leads.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Admin,Manager,Sales")]
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
