using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using System.Security.Claims;

namespace KutuphaneProjesi.Controllers
{
    [Authorize]
    public class LoansController : Controller
    {
        private readonly LibraryDbContext _context;

        public LoansController(LibraryDbContext context)
        {
            _context = context;
        }

        // Controllers/LoansController.cs -> Index Metodu
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var loansQuery = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.ReturnDate == null || (l.FineAmount != null && l.FineAmount > 0))
                .OrderByDescending(l => l.LoanDate)
                .AsQueryable(); // Sayfalama için Queryable'a çevirdik

            if (User.IsInRole("User"))
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    loansQuery = loansQuery.Where(l => l.UserId == userId);
                }
            }

            int pageSize = 5; // Her sayfada 5 ödünç işlemi
            return View(await PaginatedList<Loan>.CreateAsync(loansQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult Create(int? bookId)
        {
            if (bookId == null) return RedirectToAction("Index", "Books");

            var book = _context.Books.Find(bookId);
            if (book == null || book.StockQuantity <= 0)
            {
                TempData["Error"] = "Stok yok!";
                return RedirectToAction("Index", "Books");
            }

            ViewBag.BookTitle = book.Title;

            if (User.IsInRole("Admin") || User.IsInRole("Personel"))
            {
                var userList = _context.Users.Where(u => u.IsActive == true)
                    .Select(u => new { Id = u.Id, Display = u.FirstName + " " + u.LastName + " (" + u.Email + ")" })
                    .ToList();
                ViewData["UserId"] = new SelectList(userList, "Id", "Display");
            }

            // 1 Dakika Sonrasına Ayarlı (Hoca için Test Modu)
            var model = new Loan { BookId = bookId.Value, DueDate = DateTime.Now.AddMinutes(1) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            if (User.IsInRole("User"))
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null) { loan.UserId = int.Parse(userIdClaim.Value); loan.Status = "Bekliyor"; }
                else return RedirectToAction("Login", "Account");
            }
            else loan.Status = "Aktif";

            var book = await _context.Books.FindAsync(loan.BookId);
            if (ModelState.IsValid && book != null && book.StockQuantity > 0)
            {
                loan.LoanDate = DateTime.Now;
                _context.Loans.Add(loan);
                book.StockQuantity--;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                if (loan.Status == "Bekliyor") TempData["Success"] = "Talep alındı.";
                else TempData["Success"] = "Ödünç verildi.";

                if (User.IsInRole("User")) return RedirectToAction("Index", "Profile");
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> ApproveLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null && loan.Status == "Bekliyor") { loan.Status = "Aktif"; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> RejectLoan(int id)
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan != null && loan.Status == "Bekliyor")
            {
                if (loan.Book != null) { loan.Book.StockQuantity++; _context.Books.Update(loan.Book); }
                _context.Loans.Remove(loan); await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null) return NotFound();

            loan.ReturnDate = DateTime.Now;
            loan.Status = "İade Edildi";

            if (loan.DueDate.HasValue && loan.ReturnDate.HasValue)
            {
                var lateSpan = loan.ReturnDate.Value - loan.DueDate.Value;
                // Dakika/Saniye farkı olsa bile ceza kes (Math.Ceiling ile yukarı yuvarla)
                if (lateSpan.TotalSeconds > 0) loan.FineAmount = (decimal)Math.Ceiling(lateSpan.TotalDays) * 1.0m;
            }

            if (loan.Book != null) { loan.Book.StockQuantity++; _context.Books.Update(loan.Book); }
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> PayFine(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null && (loan.FineAmount != null && loan.FineAmount > 0))
            {
                loan.FineAmount = 0; // Borcu Sil
                _context.Loans.Update(loan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ceza tahsil edildi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}