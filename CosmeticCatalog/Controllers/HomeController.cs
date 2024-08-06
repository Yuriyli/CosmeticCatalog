using CosmeticCatalog.Services;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CosmeticCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CatalogService _catalog;

        public HomeController(ILogger<HomeController> logger, CatalogService catalog)
        {
            _logger = logger;
            _catalog = catalog;
        }

        [Route("")]
        public IActionResult Index(int? id)
        {
            return View(id);
        }

        [Route("Product")]
        public IActionResult Product()
        {
            return View("Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
