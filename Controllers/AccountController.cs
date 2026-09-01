using RealEstateSystem.Models;
using RealEstateSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = userManager.CreateAsync(user, model.Password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, model.Role).Wait();
                    signInManager.SignInAsync(user, false).Wait();
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = userManager.FindByEmailAsync(model.Email).Result;

                if (user != null)
                {
                    bool correctPassword = userManager.CheckPasswordAsync(user, model.Password).Result;

                    if (correctPassword)
                    {
                        signInManager.SignInAsync(user, model.RememberMe).Wait();
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
