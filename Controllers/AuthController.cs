using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Models;

namespace OnlineMarket.Controllers;
public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Parolni to'g'ri takrorlang");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth
            };



            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the "Client" role to the new user
                await _userManager.AddToRoleAsync(user, "Client");
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Message"] = "Ro'yhatdan o'tdingiz";
                return RedirectToAction("Index", "Product");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Foydalanuvchi mavjud emas!");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);


            if (result.Succeeded)
            {
                var customClaims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) };
                await _signInManager.SignInWithClaimsAsync(user, false, customClaims);


                TempData["Message"] = "Tizimga xush kelibsiz!";

                return RedirectToAction("Index", "Product");
            }


            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Message"] = "Tizimdan chiqdingiz";

        return RedirectToAction("Index", "Product");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
