using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPSAssetAdmin.Data;
using System.Text;

namespace PPSAssetAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LeadController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public LeadController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string project, string utmSource, DateTime? startDate, DateTime? endDate)
        {
            // Populate Project Dropdown from sy_project table
            var projects = await _context.Projects
                                         .Where(p => !string.IsNullOrEmpty(p.ProjectName))
                                         .OrderBy(p => p.ProjectName)
                                         .Select(p => p.ProjectName)
                                         .ToListAsync();
            
            ViewBag.Projects = projects;

            var leads = from l in _context.Transactions select l;

            if (!string.IsNullOrEmpty(searchString))
            {
                leads = leads.Where(s => (s.FirstName != null && s.FirstName.Contains(searchString)) || 
                                         (s.LastName != null && s.LastName.Contains(searchString)) || 
                                         (s.Phone != null && s.Phone.Contains(searchString)) || 
                                         (s.Email != null && s.Email.Contains(searchString)));
            }
            if (!string.IsNullOrEmpty(project))
            {
                leads = leads.Where(x => x.Project == project);
            }
            if (!string.IsNullOrEmpty(utmSource))
            {
                leads = leads.Where(x => x.UtmSource == utmSource);
            }
            if (startDate.HasValue)
            {
                leads = leads.Where(x => x.RegisterDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // Include the whole end day
                leads = leads.Where(x => x.RegisterDate < endDate.Value.AddDays(1));
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentProject"] = project;
            ViewData["CurrentUtmSource"] = utmSource;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(await leads.OrderByDescending(x => x.RegisterDate).ToListAsync());
        }

        public async Task<IActionResult> Export()
        {
            var leads = await _context.Transactions.OrderByDescending(x => x.RegisterDate).ToListAsync();
            var builder = new StringBuilder();
            builder.AppendLine("Id,FirstName,LastName,Phone,Email,Project,UtmSource,UtmMedium,UtmCampaign,Date");

            foreach (var lead in leads)
            {
                builder.AppendLine($"{lead.Id},{Escape(lead.FirstName)},{Escape(lead.LastName)},{Escape(lead.Phone)},{Escape(lead.Email)},{Escape(lead.Project)},{Escape(lead.UtmSource)},{Escape(lead.UtmMedium)},{Escape(lead.UtmCampaign)},{lead.RegisterDate}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"leads_{DateTime.Now:yyyyMMddHHmm}.csv");
        }

        private string Escape(string? term)
        {
            if (term == null) return "";
            return term.Contains(",") ? $"\"{term}\"" : term;
        }
    }
}
