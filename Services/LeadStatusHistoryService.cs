using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LMS_Backend.Services
{
    public class LeadStatusHistoryService
    {
        private readonly ApplicationDbContext _context;

        public LeadStatusHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogStatusChange(int leadId, string oldStatus, string newStatus, int changedBy)
        {
            var leadStatusHistory = new LeadStatusHistory
            {
                LeadId = leadId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedBy = changedBy
            };

            _context.LeadStatusHistories.Add(leadStatusHistory);
            await _context.SaveChangesAsync();
        }
    }
}
