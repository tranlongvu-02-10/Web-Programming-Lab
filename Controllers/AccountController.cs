using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab04.WebsiteBanHang.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab04.WebsiteBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            var allowedRoles = new List<string> { SD.Role_Customer, SD.Role_Employee };
            var roles = await _roleManager.Roles
                .Where(r => allowedRoles.Contains(r.Name))
                .Select(r => new { r.Name })
                .ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Khai báo allowedRoles ở ngoài scope để sử dụng ở cả hai nơi
            var allowedRoles = new List<string> { SD.Role_Customer, SD.Role_Employee };

            if (ModelState.IsValid)
            {
                if (model.Age.HasValue && (model.Age.Value < 17 || model.Age.Value > 100))
                {
                    ModelState.AddModelError("Age", "Tuổi phải từ 17 đến 100");
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                }

                if (!allowedRoles.Contains(model.SelectedRole) || !await _roleManager.RoleExistsAsync(model.SelectedRole))
                {
                    ModelState.AddModelError("SelectedRole", "Vai trò không hợp lệ hoặc không được phép.");
                }

                if (!ModelState.IsValid)
                {
                    var roles = await _roleManager.Roles
                        .Where(r => allowedRoles.Contains(r.Name))
                        .Select(r => new { r.Name })
                        .ToListAsync();
                    ViewBag.Roles = new SelectList(roles, "Name", "Name", model.SelectedRole);
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    Age = model.Age
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("Password", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                var rolesOnError = await _roleManager.Roles
                    .Where(r => allowedRoles.Contains(r.Name))
                    .Select(r => new { r.Name })
                    .ToListAsync();
                ViewBag.Roles = new SelectList(rolesOnError, "Name", "Name", model.SelectedRole);
            }

            // Gán ViewBag.Roles khi ModelState không hợp lệ ngay từ đầu
            var rolesOnInvalid = await _roleManager.Roles
                .Where(r => allowedRoles.Contains(r.Name))
                .Select(r => new { r.Name })
                .ToListAsync();
            ViewBag.Roles = new SelectList(rolesOnInvalid, "Name", "Name", model.SelectedRole);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ManageViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                Age = user.Age ?? 17
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.FullName))
                {
                    ModelState.AddModelError("FullName", "Họ và tên không được để trống.");
                    model.Email = user.Email;
                    return View(model);
                }

                if (model.Age.HasValue && (model.Age.Value < 17 || model.Age.Value > 100))
                {
                    ModelState.AddModelError("Age", "Tuổi phải từ 17 đến 100.");
                    model.Email = user.Email;
                    return View(model);
                }

                user.FullName = model.FullName;
                user.Address = model.Address;
                user.Age = model.Age;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction(nameof(Manage));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Email = user.Email;
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}