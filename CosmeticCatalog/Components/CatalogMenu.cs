﻿using CosmeticCatalog.Models;
using CosmeticCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Components
{
    public class CatalogMenu : ViewComponent
    {
        private readonly CatalogService _catalogService;

        public CatalogMenu(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id, string actionString = "index")
        {
            ViewBag.Action = actionString;
            var categories = await _catalogService.GetCategoriesMenuVMAsync(id);
            return View(categories);
        }
    }
}
