using KutuphaneProjesi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace KutuphaneProjesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;

        public HomeController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // ÝSTATÝSTÝKLER (Sadece Yetkililer Görür)
            if (User.IsInRole("Admin") || User.IsInRole("Personel"))
            {
                // 1. Toplam Kitap (Silinmemiþ olanlar)
                ViewBag.TotalBooks = _context.Books.Count(b => b.IsDeleted == false);

                // 2. Normal Üye Sayýsý (Sadece Okuyucular)
                ViewBag.TotalMembers = _context.Users.Count(u => u.Role == "User" && u.IsActive == true);

                // 3. Personel/Admin Sayýsý (Yöneticiler)
                ViewBag.TotalStaff = _context.Users.Count(u => (u.Role == "Admin" || u.Role == "Personel") && u.IsActive == true);

                // 4. Aktif Ödünçler (Dýþarýdaki Kitaplar)
                ViewBag.ActiveLoans = _context.Loans.Count(l => l.ReturnDate == null && l.Status == "Aktif");

                // 5. Onay Bekleyen Talepler
                ViewBag.PendingLoans = _context.Loans.Count(l => l.Status == "Bekliyor");

                // 6. Toplam Tahsil Edilen/Biriken Ceza (Kasa)
                ViewBag.TotalFines = _context.Loans.Sum(l => l.FineAmount ?? 0);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}