using CosmeticCatalog.Services;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CategoryListMenu : ViewComponent
    {
        private readonly CatalogService _catalogService;

        public CategoryListMenu(CatalogService catalogService)
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
