using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using PPSAssetAdmin.Data;
using PPSAssetAdmin.Models;

namespace PPSAssetAdmin.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SyUser?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.SyUsers.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();
                return user;
            }
            return null;
        }
        
        public async Task<SyUser?> CreateUserAsync(string username, string password, string? displayName, string role, bool isActive = true)
        {
             if (await _context.SyUsers.AnyAsync(u => u.Username == username))
                return null;

            var user = new SyUser
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                DisplayName = displayName ?? string.Empty,
                Role = role,
                IsActive = isActive
            };
            
            _context.SyUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PPSAssetAdmin.Areas.Admin.Models.DashboardViewModel> GetDashboardStatsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(t => t.RegisterDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Ensure we include the whole end day
                var actualEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.RegisterDate <= actualEndDate);
            }

            var total = await query.CountAsync();
            
            // These stay explicitly relative to "Now" regardless of filter
            var today = DateTime.Today;
            var leadsToday = await _context.Transactions.CountAsync(t => t.RegisterDate >= today);
            
            var weekAgo = today.AddDays(-7);
            var leadsThisWeek = await _context.Transactions.CountAsync(t => t.RegisterDate >= weekAgo);

            // Leads by Project - Filtered
            var leadsByProject = await query
                .Where(t => t.Project != null)
                .GroupBy(t => t.Project)
                .Select(g => new PPSAssetAdmin.Areas.Admin.Models.ChartData 
                { 
                    Label = g.Key ?? "Unknown", 
                    Value = g.Count() 
                })
                .ToListAsync();

            // Leads Sort by week
            // If filtered, use the filtered query. If not, default to last 8 weeks.
            IQueryable<PPSAssetAdmin.Models.TrTransaction> weekQuery = query;
            
            if (!startDate.HasValue && !endDate.HasValue)
            {
                var eightWeeksAgo = today.AddDays(-56);
                weekQuery = weekQuery.Where(t => t.RegisterDate >= eightWeeksAgo);
            }

            var rawLeads = await weekQuery
                .Select(t => t.RegisterDate)
                .ToListAsync();

            var leadsByWeek = rawLeads
                .GroupBy(d => System.Globalization.ISOWeek.GetYear(d) * 100 + System.Globalization.ISOWeek.GetWeekOfYear(d))
                .Select(g => {
                     var firstItem = g.First();
                     // Calculate start of week (Monday)
                     var diff = (7 + (firstItem.DayOfWeek - DayOfWeek.Monday)) % 7;
                     var startOfWeek = firstItem.AddDays(-1 * diff).Date;
                     
                     return new PPSAssetAdmin.Areas.Admin.Models.ChartData
                     {
                         Label = startOfWeek.ToString("d MMM"),
                         Value = g.Count(),
                         Timestamp = startOfWeek
                     };
                })
                .OrderBy(x => x.Timestamp)
                .Select(x => new PPSAssetAdmin.Areas.Admin.Models.ChartData { Label = x.Label, Value = x.Value })
                .ToList();
            
            return new PPSAssetAdmin.Areas.Admin.Models.DashboardViewModel
            {
                TotalLeads = total,
                LeadsToday = leadsToday,
                LeadsThisWeek = leadsThisWeek,
                StartDate = startDate,
                EndDate = endDate,
                LeadsByProject = leadsByProject,
                LeadsByWeek = leadsByWeek
            };
        }
    }
}
