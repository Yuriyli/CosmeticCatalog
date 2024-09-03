using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CategorySelector : ViewComponent
    {
        private readonly CatalogService _catalog;

        public CategorySelector(CatalogService catalog)
        {
            _catalog = catalog;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? parentId)
        {
            ViewBag.ParentId = parentId;
            var model = await _catalog.GetCategoriesMenuVMAsync(parentId);
            return View(model);
        }
    }
}
