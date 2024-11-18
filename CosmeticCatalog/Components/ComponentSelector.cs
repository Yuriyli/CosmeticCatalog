using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class ComponentSelector : ViewComponent
    {
        private readonly CatalogService _catalog;

        public ComponentSelector(CatalogService catalogService)
        {
            _catalog = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<int> componentIds)
        {
            ViewBag.Components = await _catalog.GetComponentsAsync();
            return View(componentIds);
        }
    }
}
