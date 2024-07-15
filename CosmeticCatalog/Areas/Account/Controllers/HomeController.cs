using CosmeticCatalog.Models;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace CosmeticCatalog.Areas.Account.Controllers
{
    /// <summary>
    /// Контроллер для регистрации, редактирования и получения информации оп ользователе
    /// </summary>
    [Area("Account")]
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize]
        [Route("{area}")]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var userVM = new UserVM
                {
                    Name = user.UserName ?? "NoName",
                    Email = user.Email ?? "NoEmail"
                };
                return View(userVM);
            }
            return new NotFoundResult();
        }

        [Route("{area}/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("{area}/Register")]
        [HttpPost]
        public async Task<IActionResult> Register(UserCreateVM userModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = userModel.Name,
                    Email = userModel.Email
                };
                IdentityResult result = await userManager.CreateAsync(appUser, userModel.Password);

                if (result.Succeeded) return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(userModel);
        }
    }
}
