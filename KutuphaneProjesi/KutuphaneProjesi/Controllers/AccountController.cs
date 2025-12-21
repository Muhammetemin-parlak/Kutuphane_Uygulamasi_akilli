using Microsoft.AspNetCore.Mvc;
using KutuphaneProjesi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace KutuphaneProjesi.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryDbContext _context;

        public AccountController(LibraryDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------------
        // GİRİŞ (LOGIN) METOTLARI
        // ------------------------------------------------------------------

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Kullanıcıyı bul
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                // IsActive kontrolü
                if (user != null && user.IsActive == true)
                {
                    // Şifre kontrolü (Basit hash karşılaştırması)
                    if (user.PasswordHash == model.Password)
                    {
                        // Rol kontrolü: Rol boşsa "User" ata
                        string userRole = !string.IsNullOrEmpty(user.Role) ? user.Role : "User";

                        // Kimlik bilgilerini (Claims) hazırla
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email ?? ""),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim(ClaimTypes.Role, userRole),
                            new Claim("UserId", user.Id.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Çerezi oluştur ve giriş yap
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        // Eğer kullanıcı özel bir sayfaya gitmeye çalışırken login'e düştüyse, oraya geri gönder
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        // --- YENİ YÖNLENDİRME MANTIĞI ---

                        if (userRole == "Personel")
                        {
                            // Personel direkt işinin başına (Ödünç Ekranına) gitsin
                            return RedirectToAction("Index", "Loans");
                        }
                        else if (userRole == "Admin")
                        {
                            // Admin ana sayfaya (veya Users sayfasına)
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Normal kullanıcılar kitaplara
                            return RedirectToAction("Index", "Books");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Geçersiz e-posta, şifre veya hesabınız pasif.");
            }

            return View(model);
        }

        // ------------------------------------------------------------------
        // ÇIKIŞ (LOGOUT) METODU
        // ------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ------------------------------------------------------------------
        // KAYIT (REGISTER) METOTLARI
        // ------------------------------------------------------------------

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = model.Password,
                    FirstName = "Yeni",
                    LastName = "Kullanıcı",
                    CreatedAt = DateTime.Now,
                    IsActive = true, // Yeni üyeler aktif olsun

                    // DÜZELTME 3: RoleId yerine Role string alanı kullanıldı
                    Role = "User"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // ------------------------------------------------------------------
        // ADMIN YAPMA METODU
        // ------------------------------------------------------------------

        public async Task<IActionResult> MakeAdmin(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Content("Lütfen bir email adresi giriniz.", "text/plain");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Content($"HATA: '{email}' adresiyle bir kullanıcı bulunamadı.", "text/plain");
            }

            // DÜZELTME 4: RoleId yerine Role string alanı güncellendi
            user.Role = "Admin";

            await _context.SaveChangesAsync();

            return Content($"BAŞARILI: Kullanıcı ({email}) artık Admin rolündedir. Çıkış yapıp tekrar giriş yapın.", "text/plain");
        }
    }
}