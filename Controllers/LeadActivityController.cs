using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_Backend.Models.Entities;
using LMS_Backend.Models;
using LMS_Backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeadActivityController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        // GET: api/<LeadActivityController>  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadActivity>>> GetLeadActivities()
        {
            return await _context.LeadActivities.ToListAsync();
        }

        // GET api/<LeadActivityController>/5  
        [HttpGet("{id}")]
        public async Task<ActionResult<LeadActivity>> GetLeadActivity(int id)
        {
            var leadActivity = await _context.LeadActivities.FindAsync(id);
            if (leadActivity == null)
            {
                return NotFound();
            }
            return leadActivity;
        }

        //Get api/<LeadActivityController>/performedby/5  
        [HttpGet("performedby/{id}")]
        public async Task<ActionResult<IEnumerable<LeadActivity>>> GetLeadActivityOfUser(int id)
        {
            var leadActivity = await _context.LeadActivities.Where(l => l.PerformedBy == id).ToListAsync();
            if (leadActivity == null || !leadActivity.Any())
            {
                return NotFound();
            }
            return leadActivity;
        }

        // POST api/<LeadActivityController>  
        [HttpPost]
        public async Task<ActionResult<LeadActivity>> PostLeadActivity([FromBody] LeadActivity leadActivity)
        {
            _context.LeadActivities.Add(leadActivity);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lead Activity Added." });
        }

        // PUT api/<LeadActivityController>/5  
        [HttpPut("{id}")]
        public async Task<ActionResult<LeadActivity>> PutLeadActivity(int id, [FromBody] LeadActivityDto leadActivity)
        {
            if (id != leadActivity.Id)
            {
                return BadRequest();
            }
            _context.Entry(leadActivity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LeadActivities.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { message = "Lead Activity Updated." });
        }

        // DELETE api/<LeadActivityController>/5  
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
