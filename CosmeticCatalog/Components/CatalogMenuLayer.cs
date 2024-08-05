using CosmeticCatalog.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CatalogMenuLayer : ViewComponent
    {
        public IViewComponentResult Invoke(Category baseCategory, List<Category> allCategories)
        {
            ViewBag.BaseCategory = baseCategory;
            ViewBag.AllCategories = allCategories;
            var hasChildren = allCategories.Any(c => c.Parent?.Id == baseCategory.Id);
            ViewBag.HasChildren = hasChildren;
            if (hasChildren)
            {
                ViewBag.Children = allCategories.Where(c => c.Parent?.Id == baseCategory.Id);
            }
            return View();
        }
    }
}
