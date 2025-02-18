using LMS_Backend.Models;
using LMS_Backend.Models.DTOs;
using LMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS_Backend.Controllers.apis.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeadStatusHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadStatusHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadStatusHistory>>> GetLeadStatusHistories()
        {
            return await _context.LeadStatusHistories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeadStatusHistory>> GetLeadStatusHistory(int id)
        {
            var leadStatusHistory = await _context.LeadStatusHistories.FindAsync(id);

            if (leadStatusHistory == null)
            {
                return NotFound();
            }

            return leadStatusHistory;
        }

        [HttpPost]
        public async Task<ActionResult<LeadStatusHistory>> PostLeadStatusHistory([FromBody] LeadStatusHistoryDto leadStatusHistoryDto)
        {
            var leadStatusHistory = new LeadStatusHistory
            {
                LeadId = leadStatusHistoryDto.LeadId,
                OldStatus = leadStatusHistoryDto.OldStatus,
                NewStatus = leadStatusHistoryDto.NewStatus,
                ChangedBy = leadStatusHistoryDto.ChangedBy
            };

            _context.LeadStatusHistories.Add(leadStatusHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeadStatusHistory), new { id = leadStatusHistory.Id }, leadStatusHistory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeadStatusHistory(int id, [FromBody] LeadStatusHistoryDto leadStatusHistoryDto)
        {
            if (id != leadStatusHistoryDto.Id)
            {
                return BadRequest();
            }

            var leadStatusHistory = new LeadStatusHistory
            {
                Id = leadStatusHistoryDto.Id,
                LeadId = leadStatusHistoryDto.LeadId,
                OldStatus = leadStatusHistoryDto.OldStatus,
                NewStatus = leadStatusHistoryDto.NewStatus,
                ChangedBy = leadStatusHistoryDto.ChangedBy
            };

            _context.Entry(leadStatusHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadStatusHistoryExists(id))
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
        public async Task<IActionResult> DeleteLeadStatusHistory(int id)
        {
            var leadStatusHistory = await _context.LeadStatusHistories.FindAsync(id);
            if (leadStatusHistory == null)
            {
                return NotFound();
            }

            _context.LeadStatusHistories.Remove(leadStatusHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeadStatusHistoryExists(int id)
        {
            return _context.LeadStatusHistories.Any(e => e.Id == id);
        }
    }
}
