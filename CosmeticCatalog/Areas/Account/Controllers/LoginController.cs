using CosmeticCatalog.Models;
using CosmeticCatalog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCatalog.Areas.Account.Controllers
{
    /// <summary>
    /// Контроллер входа выхода пользователя
    /// </summary>
    [Area("Account")]
    public class LoginController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager)
        {
            _userManager = userManager;
            _signInManager = signinManager;
        }

        [Route("{area}/Login")]
        [HttpGet]
        public IActionResult Index(string returnUrl = "/")
        {
            LoginVM login = new LoginVM();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [Route("{area}/Login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                AppUser? appUser = await _userManager.FindByNameAsync(loginVM.Name);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(
                        appUser, loginVM.Password, loginVM.Remember, false);

                    if (result.Succeeded)
                    {
                        //если пользователь прошел аутентификацию переадресует на изначальную страницу, либо на главну
                        return Redirect(loginVM.ReturnUrl ?? "/"); 
                    }
                }
                ModelState.AddModelError(nameof(loginVM.Name), "Неверное имя или пароль");
            }
            return View(loginVM);            
        }

        [Route("{area}/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home", new { area = ""});
        }
    }
}
