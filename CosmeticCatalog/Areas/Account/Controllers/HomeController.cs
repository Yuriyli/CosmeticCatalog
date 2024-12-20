﻿using CosmeticCatalog.Models;
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
        private UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [Route("{area}")]
        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var userVM = new UserVM
                {
                    Name = user.UserName ?? "NoName",
                    Email = user.Email ?? "NoEmail",
                };
                
                // Добавляет роли в VM.
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var r in roles)
                {
                    userVM.Roles.Add(r);
                }

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
        public async Task<IActionResult> RegisterAsync(UserCreateVM userModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = userModel.Name,
                    Email = userModel.Email
                };
                IdentityResult result = await _userManager.CreateAsync(appUser, userModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index","Login");
                }
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
