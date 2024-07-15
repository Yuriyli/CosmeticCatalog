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
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public LoginController(UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
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
                AppUser? appUser = await userManager.FindByNameAsync(loginVM.Name);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(
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
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
