using LMS_Backend.Controllers.apis.Authentication;
using LMS_Backend.Models;
using LMS_Backend.Models.Entities;
using LMS_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS_Backend.Services
{
    public class LeadService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LeadService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Dictionary<string, int>> GetLeadCountsByStatus()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userRoleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || userRoleClaim == null)
            {
                throw new UnauthorizedAccessException("Invalid user token");
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
                throw new UnauthorizedAccessException("Unknown role");
            }

            var leads = await query.ToListAsync();

            // Calculate lead counts by status
            var leadCounts = leads.GroupBy(l => l.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Status, g => g.Count);

            return leadCounts;
        }
    }
}
