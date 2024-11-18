using Azure;
using CosmeticCatalog.Models;
using CosmeticCatalog.Services;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class CreateController : Controller
    {
        private readonly ModeratorService _moderator;
        private readonly UserManager<AppUser> _userManager;
        private readonly CatalogService _catalog;

        public CreateController(ModeratorService moderator, UserManager<AppUser> userManager, CatalogService catalog)
        {
            _moderator = moderator;
            _userManager = userManager;
            _catalog = catalog;
        }

        #region Category

        [Route("{area}/{controller}/Category")]
        public IActionResult Category()
        {
            return View();
        }

        [Route("{area}/{controller}/Category")]
        [HttpPost]
        public async Task<IActionResult> CategoryAsync(CategoryEditVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось идентифицировать пользователя");
                return View(categoryVM);
            }

            Category categotyDbModel = new Category
            {
                Name = categoryVM.Name,
                ParentId = categoryVM.ParentId
            };
            var result = await _moderator.CreateCategoryAsync(categotyDbModel, appUser);
            if (!result)
            {
                ModelState.AddModelError("Name", "ОШИБКА БД. Не удалось создать новую категорию");
                return View(categoryVM);
            }
            return View("CategorySuccess", categotyDbModel);
        }

        #endregion

        #region Component

        [Route("{area}/{controller}/Component")]
        public async Task<IActionResult> ComponentAsync()
        {
            ViewBag.Tags = await _catalog.GetTagsAsync();
            return View();
        }


        [Route("{area}/{controller}/Component")]
        [HttpPost]
        public async Task<IActionResult> ComponentAsync(ComponentEditVM component)
        {
            if (!ModelState.IsValid)
            {
                return View(component);
            }

            // Если  имя не уникалоно (без учета регистра) - возвращает невалидное состояние модели
            if (!await _moderator.IsUniqueComponentNameAsync(component.Name))
            {
                ModelState.AddModelError("Name", "Компонент с таким названием уже существует");
                return View(component);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось идентифицировать пользователя");
                return View(component);
            }

            var componentDbModel = new Component()
            {
                Name = component.Name,
                Description = component.Description
            };
            if (component.TagIds.Count > 0)
            {
                componentDbModel.Tags = await _catalog.GetTagsAsync(component.TagIds);
            }

            var result = await _moderator.CreateComponentAsync(componentDbModel, appUser);

            if (!result)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось создать новый тег.");
                return View(component);
            }
            else
            {
                return View("ComponentSuccess", componentDbModel);
            }
        }

        #endregion

        #region Product

        [Route("{area}/{controller}/Product")]
        public IActionResult Product()
        {
            return View();
        }

        [Route("{area}/{controller}/Product")]
        [HttpPost]
        public async Task<IActionResult> ProductAsync(ProductEditVM productVM)
        {
            if (!ModelState.IsValid) return View(productVM);
            if (productVM.ParentId == null)
            {
                ModelState.AddModelError("ParentId", "Необходимо выбрать категорию");
                return View(productVM);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось идентифицировать пользователя");
                return View(productVM);
            }

            var productDb = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                CategoryId = productVM.ParentId
            };
            if (productVM.ComponentIds.Count > 0)
            {
                productDb.Components = await _catalog.GetComponentsAsync(productVM.ComponentIds);
            }
            if (productVM.TagIds.Count > 0)
            {
                productDb.Tags = await _catalog.GetTagsAsync(productVM.TagIds);
            }

            var result = await _moderator.CreateProductAsync(productDb, appUser);
            if (!result)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось создать новый тег.");
                return View(productVM);
            }
            else
            {
                return View("ProductSuccess", productDb);
            }
        }
        #endregion

        #region Tag

        [Route("{area}/{controller}/Tag")]
        public IActionResult Tag()
        {
            return View();
        }

        [Route("{area}/{controller}/Tag")]
        [HttpPost]
        public async Task<IActionResult> TagAsync(TagEditVM tag)
        {
            if (!ModelState.IsValid)
            {
                return View(tag);
            }

            // Если  имя не уникалоно (без учета регистра) - возвращает невалидное состояние модели
            if (!await _moderator.IsUniqueTagNameAsync(tag.Name))
            {
                ModelState.AddModelError("Name", "Тег с таким названием уже существует");
                return View(tag);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось идентифицировать пользователя");
                return View(tag);
            }

            var tagDbModel = new Tag
            {
                Name = tag.Name
            };
            var result = await _moderator.CreateTagAsync(tagDbModel, appUser);
            if (!result)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Не удалось создать новый тег.");
                return View(tag);
            }
            else
            {
                return View("TagSuccess", tag);
            }
        }

        #endregion
    }
}
