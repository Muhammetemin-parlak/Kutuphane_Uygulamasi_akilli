using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneProjesi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneProjesi.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly LibraryDbContext _context;
        public ProfileController(LibraryDbContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .Include(u => u.Loans).ThenInclude(l => l.Book)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null) return NotFound();
            return View(user);
        }
    }
}