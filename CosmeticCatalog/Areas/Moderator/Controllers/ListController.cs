using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]    
    public class ListController : Controller
    {
        private readonly CatalogService _catalog;

        public ListController(CatalogService catalog)
        {
            _catalog = catalog;
        }

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
        public async Task<IActionResult> TagAsync()
        {
            var result = await _catalog.GetTagsAsync();
            return View(result);
        }
    }
}
