using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class HomeController : Controller
    {
        [Route("{area}")]
        [HttpGet]
        public IActionResult Index(int categoryId)
        {
            return View();
        }
    }
}
