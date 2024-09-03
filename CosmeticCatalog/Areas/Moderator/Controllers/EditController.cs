using Azure;
using CosmeticCatalog.Models;
using CosmeticCatalog.Services;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CosmeticCatalog.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator")]
    public class EditController : Controller
    {
        private readonly ModeratorService _moderator;
        private readonly CatalogService _catalog;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditController> _logger;

        public EditController(ModeratorService moderator, CatalogService catalog, UserManager<AppUser> userManager, ILogger<EditController> logger)
        {
            _moderator = moderator;
            _catalog = catalog;
            _userManager = userManager;
            _logger = logger;
        }

        #region Category

        [Route("{area}/{controller}/Category")]
        public async Task<IActionResult> CategoryAsync(int id)
        {
            var categoryDb = await _catalog.GetCategoryAsync(id);
            if (categoryDb == null) return new NotFoundResult();

            var result = new CategoryEditVM
            {
                Id = categoryDb.Id,
                Name = categoryDb.Name,
                ParentId = categoryDb.ParentId
            };

            return View(result);
        }

        [Route("{area}/{controller}/Category")]
        [HttpPost]
        public async Task<IActionResult> CategoryAsync(CategoryEditVM categoryVM)
        {
            if (!ModelState.IsValid) return View(categoryVM);
            var categoryDb = await _catalog.GetCategoryAsync(categoryVM.Id);
            if (categoryDb == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА БД. Не удалось загрузить категорию.");
                return View(categoryVM);
            }

            if(categoryVM.Id == categoryVM.ParentId)
            {
                ModelState.AddModelError("Name", "ОШИБКА. Невозможно вложить категорию в саму себя.");
                return View(categoryVM);
            }
            if (categoryVM.Name == categoryDb.Name && categoryVM.ParentId == categoryDb.ParentId)
            {
                return View(categoryVM);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА, пользователь не идентифицирован");
                return View(categoryVM);
            }

            var resultCategory = new Category
            {
                Id = categoryVM.Id,
                Name = categoryVM.Name,
                ParentId = categoryVM.ParentId
            };

            var result = await _moderator.UpdateCategoryAsync(resultCategory, appUser);
            if (!result)
            {
                ModelState.AddModelError("Name", "ОШИБКА БД. Не удалось обновить категорию.");
                return View(categoryVM);
            }
            return View("CategorySuccess", resultCategory);
        }

        #endregion

        #region Component

        [Route("{area}/{controller}/Component")]
        public async Task<IActionResult> ComponentAsync(int? id)
        {
            if (id == null) return new NotFoundResult();
            var component = await _catalog.GetComponentAsync((int)id);
            if (component == null) return new NotFoundResult();
            var result = new ComponentEditVM()
            {
                Id = component.Id,
                Name = component.Name,
                Description = component.Description
            };
            foreach (var t in component.Tags)
            {
                result.TagIds.Add(t.Id);
            }
            ViewBag.IsDeletable = await _moderator.ComponentIsDeletableAsync((int)id);
            return View(result);
        }

        [Route("{area}/{controller}/Component")]
        [HttpPost]
        public async Task<IActionResult> ComponentAsync(ComponentEditVM component)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsDeletable = await _moderator.ComponentIsDeletableAsync(component.Id);
                return View(component);
            }
            var originalComponent = await _catalog.GetComponentAsync(component.Id);
            if (originalComponent == null) return new NotFoundResult();



            // Если имя не совпадает без учета регистра и уникально
            if (!originalComponent.Name.Equals(component.Name, StringComparison.CurrentCultureIgnoreCase)
                && !await _moderator.IsUniqueComponentNameAsync(component.Name))
            {
                ModelState.AddModelError("Name", "Компонент с таким названием уже существует");
                ViewBag.IsDeletable = await _moderator.ComponentIsDeletableAsync(component.Id);
                return View(component);
            }
            // Если имя свопадает проверить есть ли изменения в других полях, включая теги
            if (originalComponent.Name == component.Name)
            {
                bool isChanged = false;
                if (originalComponent.Description != component.Description) isChanged = true;
                foreach (var t in originalComponent.Tags)
                {
                    var isCont = component.TagIds.Contains(t.Id);
                    if (!isCont) isChanged = true;
                }
                foreach (var tId in component.TagIds)
                {
                    var t = originalComponent.Tags.FirstOrDefault(t => t.Id == tId);
                    if (t == null) isChanged = true;
                }
                if (!isChanged) return RedirectToAction("Component", new { id = component.Id });
            }
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                ModelState.AddModelError("Name", "ОШИБКА, пользователь не идентифицирован");
                ViewBag.IsDeletable = await _moderator.ComponentIsDeletableAsync(component.Id);
                return View(component);
            }

            var componentDbModel = new Component
            {
                Id = component.Id,
                Name = component.Name,
                Description = component.Description
            };

            if (component.TagIds.Count == 0)
            {
                componentDbModel.Tags = new List<Tag>();
            }
            else componentDbModel.Tags = await _catalog.GetTagsAsync(component.TagIds);

            var result = await _moderator.UpdateComponentAsync(componentDbModel, appUser);

            if (result)
            {
                return View("ComponentSuccess", componentDbModel);
            }
            else
            {
                ModelState.AddModelError("Name", "ОШИБКА БД, не удалось сохранить изменения");
                ViewBag.IsDeletable = await _moderator.ComponentIsDeletableAsync(component.Id);
                return View(component);
            }
        }

        [Route("{area}/{controller}/ComponentDelete")]
        [HttpPost]
        public async Task<IActionResult> ComponentDeleteAsync(int id)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return new NotFoundResult();

            var component = await _catalog.GetComponentAsync(id);
            if (component == null) return new NotFoundResult();

            var result = await _moderator.DeleteComponentAsync(id, appUser);
            if (result) return View("ComponentDeleteSuccess", component.Name);
            return new NotFoundResult();
        }

        #endregion

        #region Product

        [Route("{area}/{controller}/Product")]
        public IActionResult Product()
        {
            return View();
        }

        #endregion

        #region Tag        

        [Route("{area}/{controller}/Tag")]
        public async Task<IActionResult> TagAsync(int id)
        {
            var result = await _moderator.GetTagEditVMAsync(id);
            if (result == null) return new NotFoundResult();
            return View(result);
        }

        [Route("{area}/{controller}/Tag")]
        [HttpPost]
        public async Task<IActionResult> TagAsync(TagEditVM tag)
        {
            var origModel = await _moderator.GetTagEditVMAsync(tag.Id);
            tag.OriginalName = origModel!.OriginalName;

            if (!ModelState.IsValid) return View(tag);

            if (tag.OriginalName == tag.Name)
            {
                ModelState.AddModelError("Name", "Новое название не может со совпадать");
                return View(tag);
            }

            // Если имя не совпадает без учета регистра и уникально
            if (!tag.OriginalName!.Equals(tag.Name, StringComparison.CurrentCultureIgnoreCase)
                && !await _moderator.IsUniqueTagNameAsync(tag.Name))
            {
                ModelState.AddModelError("Name", "Тег с таким названием уже существует");
                return View(tag);
            }

            // Если использовать поле TagType не забыть добавить поле и добавить выбор в View
            var tagDbModel = new Tag()
            {
                Id = tag.Id,
                Name = tag.Name
            };

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                _logger.LogError($"Не удалось изменить тег \"{tag.Name}\" id:{tag.Id}. Пользователь не идентифицирован");
                return new NotFoundResult();
            }

            var result = await _moderator.UpdateTagAsync(tagDbModel, appUser);
            if (!result)
            {
                _logger.LogError($"Не удалось сохранить изменения в теге \"{tag.Name}\" id:{tag.Id}.");
                return new NotFoundResult();
            }
            return View("TagSuccess", tag);
        }

        [Route("{area}/{controller}/TagDelete")]
        [HttpPost]
        public async Task<IActionResult> TagDeleteAsync(int id)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return new NotFoundResult();

            var tag = await _moderator.GetTagEditVMAsync(id);
            if (tag == null) return new NotFoundResult();

            var result = await _moderator.DeleteTagAsync(id, appUser);
            if (result) return View("TagDeleteSuccess", tag.Name);
            return new NotFoundResult();
        }

        #endregion
    }
}
