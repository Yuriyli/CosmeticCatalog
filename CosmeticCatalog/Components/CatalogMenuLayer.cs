﻿using CosmeticCatalog.Models;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CatalogMenuLayer : ViewComponent
    {
        public IViewComponentResult Invoke(CategoryMenuVM baseCategory, List<CategoryMenuVM> allCategories, string actionString)
        {
            ViewBag.BaseCategory = baseCategory;
            ViewBag.AllCategories = allCategories;
            ViewBag.Action = actionString;

            if (baseCategory.HasChildren)
            {
                ViewBag.Children = allCategories.Where(c => c.ParentId == baseCategory.Id);
            }

            return View();
        }
    }
}
