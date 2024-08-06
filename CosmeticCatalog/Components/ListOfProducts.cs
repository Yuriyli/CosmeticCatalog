using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class ListOfProducts : ViewComponent
    {
        private readonly CatalogService _catalog;

        public ListOfProducts(CatalogService catalog)
        {
            _catalog = catalog;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var result = await _catalog.GetSimpleProductListFromCategotyAsync(id);
            ViewBag.IsModerator = User.IsInRole("Moderator");
            return View(result);
        }
    }
}
