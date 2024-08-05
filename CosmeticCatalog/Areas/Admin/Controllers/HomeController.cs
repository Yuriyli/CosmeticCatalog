using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CosmeticCatalog.Areas.Admin.Controllers
{
    /// <summary>
    /// Контроллер управления правами и инициализации прав первичного админа
    /// </summary>
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Показать список пользователей
        /// </summary>
        /// <returns></returns>
        [Route("{area}")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAsync()
        {
            // Если в проекте станет много пользователей(маловероятно), сделать пагинацию и поиск
            var appUsers = _userManager.Users;
            var usersVM = new List<UserVM>();

            foreach (var appU in appUsers)
            {
                var userVM = new UserVM()
                {
                    Id = appU.Id,
                    Name = appU.UserName,
                    Email = appU.Email
                };
                var roles = await _userManager.GetRolesAsync(appU);
                foreach (var r in roles)
                {
                    userVM.Roles.Add(r);
                }
                usersVM.Add(userVM);
            }

            return View(usersVM);
        }

        /// <summary>
        /// Доступно только авторизованным пользователям.
        /// Инициализация ролей,если их не существует.
        /// Проверяет есть ли пользователь с ролью Admin, если нет возвращает View предложением
        /// </summary>
        /// <returns></returns>
        [Route("{area}/Getadminrole")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdminRoleAsync()
        {
            // Проверки наличия ролей в бд, если нет создает их, если не удалось создать возвращает 404
            if (!(await _roleManager.RoleExistsAsync("Admin")))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (result.Succeeded) _logger.LogInformation("Создана роль \"Admin\"");
                else return new NotFoundResult();
            }
            if ((!await _roleManager.RoleExistsAsync("Moderator")))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole("Moderator"));
                if (result.Succeeded) _logger.LogInformation("Создана роль \"Moderator\"");
                else return new NotFoundResult();
            }

            // Проверяет получил ли кто-то роль Admin, если да возвращает 404
            var role = await _roleManager.FindByNameAsync("Admin");
            if (role == null) return new NotFoundResult();
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.RoleId == role.Id);
            if (userRole != null)
            {
                return new NotFoundResult();
            }

            // Если все проверки пройдены возвращает View где можно получить права
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        /// <summary>
        /// Получение прав администратора при нажатие кнопки в \Admin\Views\Home\GetAdminRole.cshtml
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Перенаправляет в случае успеха на страницу Account/Index</returns>
        [Route("{area}/Getadminrole")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAdminRoleAsync(string userId)
        {
            // проверки на наличие ролей в бд
            if (!(await _roleManager.RoleExistsAsync("Admin"))) return new NotFoundResult();
            if (!(await _roleManager.RoleExistsAsync("Moderator"))) return new NotFoundResult();

            //поиск существующих администраторов
            var role = await _roleManager.FindByNameAsync("Admin");
            if (role == null) return new NotFoundResult();
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.RoleId == role.Id);
            if (userRole != null) return new NotFoundResult();

            //Присвоение ролей пользователю из запроса
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new NotFoundResult();
            var roles = new[] { "Admin", "Moderator" };
            var result = await _userManager.AddToRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                {
                    _logger.LogCritical(e.Description);
                }
                return new NotFoundResult();
            }
            else
            {
                string info = "Пользователю - ";
                info += user.UserName + " ";
                info += "ID: " + user.Id + " ";
                info += " добавлены роли Admin, Moderator";
                _logger.LogInformation(info);
                return RedirectToAction("Index", "Account");
            }
        }

        /// <summary>
        /// Информация о пользователе и выдача прав
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        [Route("{area}/EditUser")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserAsync(string userId)
        {
            var result = new UserVM();
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null) return new NotFoundResult();

            result.Id = appUser.Id;
            result.Name = appUser.UserName;
            result.Email = appUser.Email;

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var r in roles)
            {
                result.Roles.Add(r);
            }

            return View(result);
        }


        /// <summary>
        /// Добавляет роль модератора пользователю
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Возвращает на страницу редактирования</returns>
        [Route("{area}/GiveModeratorRole")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GiveModeratorRoleAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null) return new NotFoundResult();

            var isInRole = await _userManager.IsInRoleAsync(appUser, "Moderator");
            if (isInRole)
            {
                _logger.LogCritical($"Невозможно назначить на роль. Пользователь {appUser.UserName} уже имеет роль модератора");
                return new BadRequestResult();
            }

            var result = await _userManager.AddToRoleAsync(appUser, "Moderator");
            if (result.Succeeded)
            {
                string text = String.Empty;
                text += "Пользователю " + appUser.UserName + "(" + appUser.Id + ") добавлена роль модератора";
                _logger.LogInformation(text);
            }

            return RedirectToAction("EditUser", new { userId = userId });
        }

        /// <summary>
        /// Забирает роль модератора
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Возвращает на страницу редактирования</returns>
        [Route("{area}/RemoveModeratorRole")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveModeratorRoleAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null) return new NotFoundResult();

            var isInRole = await _userManager.IsInRoleAsync(appUser, "Moderator");
            if (!isInRole)
            {
                _logger.LogCritical($"Невозможно лишить роли пользователя {appUser.UserName}, роль отсутсвует");
                return new BadRequestResult();
            }

            var result = await _userManager.RemoveFromRoleAsync(appUser, "Moderator");
            if (result.Succeeded)
            {
                string text = String.Empty;
                text += "У пользователя " + appUser.UserName + "(" + appUser.Id + ") убрана роль модератора";
                _logger.LogInformation(text);
            }

            return RedirectToAction("EditUser", new { userId = userId });
        }

    }
}


