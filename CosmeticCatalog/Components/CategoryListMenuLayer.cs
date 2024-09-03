using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CategoryListMenuLayer : ViewComponent
    {
        public IViewComponentResult Invoke(CategoryMenuVM baseCategory, List<CategoryMenuVM> allCategories)
        {
            ViewBag.BaseCategory = baseCategory;
            ViewBag.AllCategories = allCategories;

            if (baseCategory.HasChildren)
            {
                ViewBag.Children = allCategories.Where(c => c.ParentId == baseCategory.Id);
            }

            return View();
        }
    }
}
