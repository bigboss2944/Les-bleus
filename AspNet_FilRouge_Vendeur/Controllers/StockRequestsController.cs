using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge_Vendeur.Models;
using AspNet_FilRouge_Vendeur.Services;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize]
    public class StockRequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocalDbService _localDb;

        public StockRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, LocalDbService localDb)
        {
            db = context;
            _userManager = userManager;
            _localDb = localDb;
        }

        private bool CanManageRequest(StockRequest request, string? currentUserId, bool isAdmin)
        {
            return isAdmin || request.RequestedById == currentUserId;
        }

        private async Task PopulateBicycleNamesViewBagAsync()
        {
            var bicycleTypes = await db.Bicycles
                .Select(b => b.TypeOfBike)
                .Where(t => t != null)
                .OrderBy(t => t)
                .Distinct()
                .ToListAsync();
            ViewBag.BicycleNames = new SelectList(bicycleTypes);
        }

        // GET: StockRequests — all authenticated users
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            var query = db.StockRequests
                .Include(r => r.RequestedBy)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(r => r.RequestedById == currentUserId);
            }

            var requests = await query
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
            return View(requests);
        }

        // GET: StockRequests/Create — sellers only
        [Authorize(Roles = "Vendeur")]
        public async Task<IActionResult> Create()
        {
            await PopulateBicycleNamesViewBagAsync();
            return View();
        }

        // POST: StockRequests/Create — sellers only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendeur")]
        public async Task<IActionResult> Create([Bind("BicycleName,Quantity,Notes")] StockRequest stockRequest)
        {
            if (ModelState.IsValid)
            {
                stockRequest.RequestDate = DateTime.Now;
                stockRequest.Status = "En attente";
                stockRequest.RequestedById = _userManager.GetUserId(User);
                db.StockRequests.Add(stockRequest);
                await db.SaveChangesAsync();

                // Synchronisation immédiate vers le cache local pour que l'Admin puisse voir la demande.
                await _localDb.BulkUpsertStockRequestsAsync(new[] { stockRequest });

                return RedirectToAction("Index");
            }
            await PopulateBicycleNamesViewBagAsync();
            return View(stockRequest);
        }

        // POST: StockRequests/Approve/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Approve(int id)
        {
            StockRequest? request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();

            var requestedType = request.BicycleName?.Trim();
            if (!string.IsNullOrWhiteSpace(requestedType) && request.Quantity > 0)
            {
                var normalizedRequestedType = requestedType.ToLower();
                var bicycle = await db.Bicycles
                    .OrderBy(b => b.Id)
                    .FirstOrDefaultAsync(b =>
                        b.TypeOfBike != null &&
                        b.TypeOfBike.ToLower() == normalizedRequestedType);

                if (bicycle != null)
                {
                    bicycle.Quantity += request.Quantity;
                }
            }

            request.Status = "Approuvée";
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: StockRequests/Reject/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Reject(int id)
        {
            StockRequest? request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();
            request.Status = "Rejetée";
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: StockRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            StockRequest? request = await db.StockRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            if (!isAdmin && request.RequestedById != currentUserId) return Forbid();
            return View(request);
        }

        // GET: StockRequests/Edit/5 — sellers can edit their own pending requests, admins can edit all
        [Authorize(Roles = "Vendeur,Administrateur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            var request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (!CanManageRequest(request, currentUserId, isAdmin)) return Forbid();
            if (request.Status != "En attente") return BadRequest("Seules les demandes en attente peuvent être modifiées.");

            await PopulateBicycleNamesViewBagAsync();
            return View(request);
        }

        // POST: StockRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendeur,Administrateur")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BicycleName,Quantity,Notes")] StockRequest stockRequest)
        {
            if (id != stockRequest.Id) return BadRequest();

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            var existing = await db.StockRequests.FindAsync(id);
            if (existing == null) return NotFound();
            if (!CanManageRequest(existing, currentUserId, isAdmin)) return Forbid();
            if (existing.Status != "En attente") return BadRequest("Seules les demandes en attente peuvent être modifiées.");

            if (!ModelState.IsValid)
            {
                await PopulateBicycleNamesViewBagAsync();
                return View(stockRequest);
            }

            existing.BicycleName = stockRequest.BicycleName;
            existing.Quantity = stockRequest.Quantity;
            existing.Notes = stockRequest.Notes;

            await db.SaveChangesAsync();
            await _localDb.BulkUpsertStockRequestsAsync(new[] { existing });

            return RedirectToAction(nameof(Index));
        }

        // GET: StockRequests/Delete/5 — sellers can delete their own pending requests, admins can delete all
        [Authorize(Roles = "Vendeur,Administrateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            var request = await db.StockRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            if (!CanManageRequest(request, currentUserId, isAdmin)) return Forbid();
            if (request.Status != "En attente") return BadRequest("Seules les demandes en attente peuvent être supprimées.");

            return View(request);
        }

        // POST: StockRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendeur,Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrateur");

            var request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();
            if (!CanManageRequest(request, currentUserId, isAdmin)) return Forbid();
            if (request.Status != "En attente") return BadRequest("Seules les demandes en attente peuvent être supprimées.");

            db.StockRequests.Remove(request);
            await db.SaveChangesAsync();
            await _localDb.DeleteStockRequestAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

