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

        public CreateController(ModeratorService moderator, UserManager<AppUser> userManager)
        {
            _moderator = moderator;
            _userManager = userManager;
        }
        [Route("{area}/{controller}/Category")]
        public IActionResult Category()
        {
            return View();
        }
        [Route("{area}/{controller}/Component")]
        public IActionResult Component()
        {
            return View();
        }
        [Route("{area}/{controller}/Product")]
        public IActionResult Product()
        {
            return View();
        }

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
                return View("TagSuccess",tag);
            }
        }
    }
}
