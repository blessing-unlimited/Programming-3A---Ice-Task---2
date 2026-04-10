using IceTask_Two.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractCentral.Models;

namespace IceTask_Two.Controllers
{
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Clients.AsNoTracking().AsQueryable();

            // Simple search on name, contact person, or email
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    c.ContactPerson.ToLower().Contains(term) ||
                    (c.Email != null && c.Email.ToLower().Contains(term)));
            }

            var list = await query.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Search = search ?? "";
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients
                .AsNoTracking()
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ContactPerson,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactPerson,Email,Phone")] Client client)
        {
            if (id != client.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClientExistsAsync(client.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<bool> ClientExistsAsync(int id)
        {
            return _context.Clients.AnyAsync(e => e.Id == id);
        }
    }
}
