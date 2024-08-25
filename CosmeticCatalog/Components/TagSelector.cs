using CosmeticCatalog.Services;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class TagSelector : ViewComponent
    {
        private readonly CatalogService _catalog;

        public TagSelector(CatalogService catalog)
        {
            _catalog = catalog;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<int> tagIds)
        {
            ViewBag.Tags = (await _catalog.GetTagsAsync());
            return View(tagIds);
        }
    }
}
