using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineMarket.Models;
using Microsoft.AspNetCore.Authorization;
using OnlineMarket.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineMarket.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;


    public UserController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }
    public async Task<ActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Foydalanuvchi mavjud emas");
        }

        var model = new ProfileViewModel
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Foydalanuvchi mavjud emas");
        }

        // Update user data
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.DateOfBirth = model.DateOfBirth;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            // Refresh the user's sign-in session
            await _signInManager.RefreshSignInAsync(user);

            TempData["Message"] = "Profilingiz yangilandi!";
            return Redirect("/");
        }

        // Add errors to ModelState if the update fails
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Foydalanuvchi topilmadi");
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Parolni to'g'ri takrorlang");
            return View(model);
        }

        // Remove the current password if it exists
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            foreach (var error in removePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // Add the new password
        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
        if (addPasswordResult.Succeeded)
        {
            // Refresh sign-in to apply the updated security stamp
            await _signInManager.RefreshSignInAsync(user);

            TempData["Message"] = "Parol o'zgartirildi!";
            return Redirect("/");
        }

        foreach (var error in addPasswordResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);

    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageUsers()
    {
        var usersInRoles = await (from user in _userManager.Users
                                  join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                  join role in _context.Roles on userRole.RoleId equals role.Id
                                  where role.Name == "Client" || role.Name == "Seller"
                                  select user).ToListAsync();
        var userRoleViewModels = new List<UserRoleViewModel>();

        foreach (var user in usersInRoles)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoleViewModels.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }

        return View(userRoleViewModels);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        // Remove the user from all roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Add the user to the selected role
        if (!await _roleManager.RoleExistsAsync(role))
        {
            return BadRequest("Ro'l mavjud emas");
        }

        await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction("ManageUsers");
    }

}