using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PPSAssetAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,User")]
    public class AdminBaseController : Controller
    {
    }
}
