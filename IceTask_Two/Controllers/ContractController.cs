using ContractCentral.Models;
using IceTask_Two.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IceTask_Two.Controllers
{
    public class ContractController : Controller
    {
        private readonly AppDbContext _context;

        public ContractController(AppDbContext context)
        {
            _context = context;
        }

        // List contracts (optional search + filters from query string)
        public async Task<IActionResult> Index(string? search, int? status, int? clientId)
        {
            // Start with all contracts and bring in client names
            var query = _context.Contract_Info
                .Include(c => c.Client)
                .AsQueryable();

            // Search in title and description (simple "contains" check)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(term) ||
                    c.Description.ToLower().Contains(term));
            }

            // Filter by status if user picked one (empty option = show all)
            if (status.HasValue && Enum.IsDefined(typeof(ContractStatus), status.Value))
            {
                var statusEnum = (ContractStatus)status.Value;
                query = query.Where(c => c.Status == statusEnum);
            }

            // Filter by client if user picked one
            if (clientId.HasValue && clientId.Value > 0)
            {
                query = query.Where(c => c.ClientId == clientId.Value);
            }

            var list = await query.OrderBy(c => c.Title).ToListAsync();

            // Remember what they typed so the form stays filled in
            ViewBag.Search = search ?? "";
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedClientId = clientId;

            var clients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
            ViewBag.ClientList = new SelectList(clients, "Id", "Name", clientId);

            return View(list);
        }

        // One contract + workflow buttons
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract_Info
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, ContractStatus newStatus)
        {
            var contract = await _context.Contract_Info.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            if (!ContractWorkflow.IsValidChange(contract.Status, newStatus))
            {
                TempData["ErrorMessage"] = "That status change is not allowed.";
                return RedirectToAction(nameof(Details), new { id = contract.Id });
            }

            contract.Status = newStatus;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Status updated.";
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        // Simple create form so you can add contracts without typing SQL
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadClientsIntoViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string description, DateTime startDate, DateTime endDate, int clientId)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("title", "Title is required.");
            }

            if (clientId <= 0)
            {
                ModelState.AddModelError("clientId", "Pick a client.");
            }

            if (!ModelState.IsValid)
            {
                await LoadClientsIntoViewBag();
                return View();
            }

            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                ModelState.AddModelError("clientId", "Client not found.");
                await LoadClientsIntoViewBag();
                return View();
            }

            var contract = new Contract
            {
                Title = title.Trim(),
                Description = description ?? "",
                StartDate = startDate,
                EndDate = endDate,
                ClientId = clientId,
                Status = ContractStatus.Draft
            };

            _context.Contract_Info.Add(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        private async Task LoadClientsIntoViewBag()
        {
            var clients = await _context.Clients.OrderBy(c => c.Name).ToListAsync();
            ViewBag.ClientId = new SelectList(clients, "Id", "Name");
        }
    }
}
