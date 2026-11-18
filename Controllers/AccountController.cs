using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemInformasiNilaiAkademik.Data;
using SistemInformasiNilaiAkademik.ViewModels;

namespace SistemInformasiNilaiAkademik.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                .FirstOrDefaultAsync(p => p.Username.ToLower() == model.Username.Trim().ToLower());

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nama pengguna atau kata sandi salah.");
                return View(model);
            }

            if (user.Status != "Aktif")
            {
                ModelState.AddModelError(string.Empty, "Akun Anda berstatus nonaktif. Silakan hubungi Administrator.");
                return View(model);
            }

            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            }
            catch
            {
                // Fallback for plaintext check in testing if hash wasn't valid BCrypt
                isPasswordValid = (model.Password == user.PasswordHash);
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Nama pengguna atau kata sandi salah.");
                return View(model);
            }

            var roleName = user.Role?.NamaRole ?? "Guru";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdPengguna.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.Nama),
                new Claim(ClaimTypes.Role, roleName)
            };

            if (user.Guru != null)
            {
                claims.Add(new Claim("GuruId", user.Guru.IdGuru.ToString()));
                claims.Add(new Claim("NipNrg", user.Guru.NipNrg));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect according to role
            return roleName switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "WaliKelas" => RedirectToAction("Index", "WaliKelas"),
                _ => RedirectToAction("Index", "Guru")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Anda telah berhasil keluar.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Pengguna
                .Include(p => p.Role)
                .Include(p => p.Guru)
                    .ThenInclude(g => g!.KelasWali)
                .FirstOrDefaultAsync(p => p.IdPengguna == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Pastikan formulir diisi dengan benar.";
                return RedirectToAction("Profile");
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Pengguna.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
            {
                TempData["ErrorMessage"] = "Kata sandi lama tidak sesuai.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kata sandi berhasil diperbarui.";
            return RedirectToAction("Profile");
        }
    }
}
