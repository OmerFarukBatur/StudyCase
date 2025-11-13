using Core.DTOs.UserDtos;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlturStudyCase.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            else
            {
                var result = await _authService.UserCreateAsync(model);

                if (!result.Status)
                {
                    ViewBag.Error = result.Message;
                    return View(model);
                }

                TempData["Success"] = result.Message;
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.Login(model);

            if (!result.Item1.Status)
            {
                ViewBag.Error = result.Item1.Message;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.Item2.Id.ToString()),
                new Claim(ClaimTypes.Name, result.Item2.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, result.Item2.LastName ?? ""),
                new Claim(ClaimTypes.Email, result.Item2.Email),
                new Claim(ClaimTypes.Role, result.Item2.Role ?? "Employee")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            if (result.Item2.Role == "Employee")
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Auth");
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpGet]
        public IActionResult PwdReset()
        {
            return View();
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpPost]
        public async Task<IActionResult> PwdReset(PasswordResetDto passwordReset)
        {
            passwordReset.Id = UserId;
            if (!ModelState.IsValid)
            {
                return View(passwordReset);
            }
            else
            {
                if (CurrentUser == null)
                    return RedirectToAction("Login", "Auth");
                else
                {                    
                    var result = await _authService.PasswordReset(passwordReset);

                    if (result.Status)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("PwdReset", "Auth");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result.Message;
                        return View(passwordReset);
                    }
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
