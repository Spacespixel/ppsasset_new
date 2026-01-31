using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPSAssetAdmin.Data;
using PPSAssetAdmin.Models;
using PPSAssetAdmin.Services;
using PPSAssetAdmin.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace PPSAssetAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly AdminService _adminService;

        public UserController(ApplicationDbContext context, AdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SyUsers.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
             if (ModelState.IsValid)
             {
                 var result = await _adminService.CreateUserAsync(model.Username, model.Password, model.DisplayName, model.Role, model.IsActive);
                 
                 if (result == null)
                 {
                     ModelState.AddModelError("Username", "Username already exists.");
                 }
                 else
                 {
                     return RedirectToAction(nameof(Index));
                 }
             }
             return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.SyUsers.FindAsync(id);
            if (user != null)
            {
                _context.SyUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
