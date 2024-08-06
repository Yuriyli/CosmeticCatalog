using CosmeticCatalog.Models;
using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CatalogMenuViewComponent : ViewComponent
    {
        private readonly CatalogService _catalogService;

        public CatalogMenuViewComponent(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            var categories = await _catalogService.GetCategoriesMenuVMAsync(id);
            return View(categories);
        }
    }
}
