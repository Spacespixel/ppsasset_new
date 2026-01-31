using Microsoft.AspNetCore.Mvc;
using PPSAssetAdmin.Services;

namespace PPSAssetAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : AdminBaseController
    {
        private readonly AdminService _adminService;

        public DashboardController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var stats = await _adminService.GetDashboardStatsAsync(startDate, endDate);
            return View(stats);
        }
    }
}
