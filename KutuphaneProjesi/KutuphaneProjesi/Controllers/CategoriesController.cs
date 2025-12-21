using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using Microsoft.AspNetCore.Authorization;

namespace KutuphaneProjesi.Controllers
{
    // DÜZELTME 1: Sadece [Authorize] dedik. (User rolü de girebilsin diye)
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly LibraryDbContext _context;

        public CategoriesController(LibraryDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // LİSTELEME (INDEX) - Herkes Görebilir
        // ---------------------------------------------------------
        // Controllers/CategoriesController.cs -> Index Metodu
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var categories = _context.Categories.AsQueryable();

            // Varsayılan sıralama: İsme göre A-Z
            categories = categories.OrderBy(c => c.Name);

            int pageSize = 5;
            return View(await PaginatedList<Category>.CreateAsync(categories.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        // ---------------------------------------------------------
        // EKLEME İŞLEMLERİ 
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // ---------------------------------------------------------
        // DÜZENLEME İŞLEMLERİ (Sadece Admin)
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // ---------------------------------------------------------
        // SİLME İŞLEMLERİ 
        // ---------------------------------------------------------
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Personel")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}