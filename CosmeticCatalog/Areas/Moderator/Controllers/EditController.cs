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
    public class EditController : Controller
    {
        private readonly ModeratorService _moderator;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditController> _logger;

        public EditController(ModeratorService moderator, UserManager<AppUser> userManager, ILogger<EditController> logger)
        {
            _moderator = moderator;
            _userManager = userManager;
            _logger = logger;
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
                ModelState.AddModelError("Name", "Новое название не может совпадать");
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
    }
}
