using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using Microsoft.AspNetCore.Authorization;

namespace KutuphaneProjesi.Controllers
{
    // [Authorize] // İstersen bu yorumu kaldırarak sayfayı sadece üyelere açabilirsin
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // 1. KİTAPLARI LİSTELE (INDEX) - DÜZELTİLDİ
        // ---------------------------------------------------------
        public async Task<IActionResult> Index(string? searchString, int? categoryId, int? pageNumber)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.IsDeleted == false)
                .AsQueryable();

            // --- Arama Filtresi (HATA DÜZELTİLDİ) ---
            if (!string.IsNullOrEmpty(searchString))
            {
                string keyword = searchString;

                // DÜZELTME: Hem Author'un kendisini hem de FullName'i kontrol ediyoruz
                booksQuery = booksQuery.Where(b =>
                    (b.Title != null && b.Title.Contains(keyword)) ||
                    (b.Author != null && b.Author.FullName != null && b.Author.FullName.Contains(keyword)));
            }

            // --- Kategori Filtresi ---
            if (categoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == categoryId);
            }

            // Filtreleri View'da koru
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categoryId);

            // Sıralama: En son eklenen en üstte
            booksQuery = booksQuery.OrderByDescending(b => b.Id);

            // --- SAYFALAMA ---
            int pageSize = 5;
            return View(await PaginatedList<Book>.CreateAsync(booksQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // ---------------------------------------------------------
        // 2. YENİ KİTAP EKLEME (GET)
        // ---------------------------------------------------------
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // ---------------------------------------------------------
        // 3. YENİ KİTAP KAYDETME (POST)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                book.IsDeleted = false;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // ---------------------------------------------------------
        // 4. DÜZENLEME (EDIT - GET)
        // ---------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // ---------------------------------------------------------
        // 5. DÜZENLEME KAYDET (EDIT - POST)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // ---------------------------------------------------------
        // 6. SİLME (DELETE)
        // ---------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.IsDeleted = true;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}