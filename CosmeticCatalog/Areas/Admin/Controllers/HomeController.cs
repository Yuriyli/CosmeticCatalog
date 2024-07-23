using CosmeticCatalog.Data;
using CosmeticCatalog.Models;
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
        public IActionResult Index()
        {
            // TODO: Создать список пользователей, пагинацию,
            // отдельные методы для просмотра отдельнх пользоавателей и выдачи прав модератора
            return View();
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
        public async Task<IActionResult> GetAdminRole()
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

        // Получение прав администратора при нажатие кнопки в \Admin\Views\Home\GetAdminRole.cshtml
        [Route("{area}/Getadminrole")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAdminRole(string userId)
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

    }
}


