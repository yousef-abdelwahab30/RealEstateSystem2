using RealEstateSystem.Models;
using RealEstateSystem.Models.ViewModels;
using RealEstateSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAgentRepository agentRepository;

        public AccountController(
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IAgentRepository _agentRepository)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            agentRepository = _agentRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    string roleName = string.IsNullOrWhiteSpace(model.Role) ? "Customer" : model.Role;

                    var roleResult = await userManager.AddToRoleAsync(user, roleName);

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                            ModelState.AddModelError("", error.Description);

                        return View(model);
                    }

                    if (roleName == "Agent")
                    {
                        Agent agent = new Agent
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            Phone = string.IsNullOrWhiteSpace(model.Phone) ? "Not set" : model.Phone,
                            AgencyName = model.FullName
                        };

                        agentRepository.Add(agent);
                        agentRepository.Save();
                    }

                    await signInManager.SignInAsync(user, isPersistent: false);

                    if (roleName == "Agent")
                        return RedirectToAction("Create", "Properties");

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
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    bool correctPassword = await userManager.CheckPasswordAsync(user, model.Password);

                    if (correctPassword)
                    {
                        await signInManager.SignInAsync(user, model.RememberMe);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

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
