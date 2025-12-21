using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneProjesi.Controllers
{
    // Personel listeyi görebilir ama rol değiştiremez/silemez
    [Authorize(Roles = "Admin,Personel")]
    public class UsersController : Controller
    {
        private readonly LibraryDbContext _context;

        public UsersController(LibraryDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // 1. KULLANICI LİSTESİ (Index) - SAYFALAMA ve ARAMA EKLENDİ
        // ---------------------------------------------------------
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var usersQuery = _context.Users.AsQueryable();

            // --- Arama Filtresi ---
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchString)) ||
                    (u.LastName != null && u.LastName.Contains(searchString)) ||
                    (u.Email != null && u.Email.Contains(searchString)));
            }

            // Arama kutusunda yazanı koru
            ViewData["CurrentFilter"] = searchString;

            // Sıralama: En son kaydolan en üstte görünsün
            usersQuery = usersQuery.OrderByDescending(u => u.CreatedAt);

            // --- SAYFALAMA AYARI ---
            int pageSize = 5; // Sayfa başına 5 kullanıcı

            return View(await PaginatedList<User>.CreateAsync(usersQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // ---------------------------------------------------------
        // 2. ROL DEĞİŞTİRME (SADECE ADMIN YAPABİLİR!)
        // ---------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")] // Personel burayı çalıştıramaz!
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                // Admin yanlışlıkla kendini yetkisiz yapmasın diye kontrol ediyoruz.
                if (User.Identity?.Name == user.Email)
                {
                    TempData["Error"] = "Kendi rolünüzü değiştiremezsiniz!";
                    return RedirectToAction(nameof(Index));
                }

                user.Role = newRole;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{user.Email} kullanıcısı artık {newRole} oldu.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        // 3. KULLANICI SİLME / PASİFE ALMA (SADECE ADMIN)
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Veriyi silmeyelim, pasif yapalım (Soft Delete)
                user.IsActive = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["Warning"] = "Kullanıcı pasife alındı.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}