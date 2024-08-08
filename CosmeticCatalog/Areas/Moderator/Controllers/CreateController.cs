using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class CreateController : Controller
    {
        [Route("{area}/{controller}/Category")]
        public IActionResult Category()
        {
            return View();
        }
        [Route("{area}/{controller}/Component")]
        public IActionResult Component()
        {
            return View();
        }
        [Route("{area}/{controller}/Product")]
        public IActionResult Product()
        {
            return View();
        }
        [Route("{area}/{controller}/Tag")]
        public IActionResult Tag()
        {
            return View();
        }
    }
}
