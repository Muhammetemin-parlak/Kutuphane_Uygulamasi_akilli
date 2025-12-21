using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using Microsoft.AspNetCore.Authorization;

namespace KutuphaneProjesi.Controllers
{
    // DÜZELTME 1: Sadece [Authorize] dedik. (User rolü de girebilsin diye)
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // LİSTELEME (INDEX) - Herkes Görebilir
        // ---------------------------------------------------------
        // Controllers/AuthorsController.cs -> Index Metodu
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var authors = _context.Authors.AsQueryable();

            // Sıralama: Adına göre A-Z
            authors = authors.OrderBy(a => a.FullName);

            int pageSize = 5;
            return View(await PaginatedList<Author>.CreateAsync(authors.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // ---------------------------------------------------------
        // YENİ EKLEME (Sadece Admin)
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")] // DÜZELTME 2: Kısıtlamayı buraya koyduk
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Create([Bind("Id,FullName,Bio")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // ---------------------------------------------------------
        // DÜZENLEME (Sadece Admin)
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Bio")] Author author)
        {
            if (id != author.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // ---------------------------------------------------------
        // SİLME (Sadece Admin)
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors.FirstOrDefaultAsync(m => m.Id == id);
            if (author == null) return NotFound();

            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}