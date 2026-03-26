using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;
using AspNet_FilRouge.Services;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class StockRequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVendorSyncService _vendorSync;

        public StockRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IVendorSyncService vendorSync)
        {
            db = context;
            _userManager = userManager;
            _vendorSync = vendorSync;
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
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

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
        [Authorize(Roles = AppConstants.Roles.Vendeur)]
        public async Task<IActionResult> Create()
        {
            await PopulateBicycleNamesViewBagAsync();
            return View();
        }

        // POST: StockRequests/Create — sellers only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Vendeur)]
        public async Task<IActionResult> Create([Bind("BicycleName,Quantity,Notes")] StockRequest stockRequest)
        {
            if (ModelState.IsValid)
            {
                stockRequest.RequestDate = DateTime.Now;
                stockRequest.Status = "En attente";
                stockRequest.RequestedById = _userManager.GetUserId(User);
                db.StockRequests.Add(stockRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            await PopulateBicycleNamesViewBagAsync();
            return View(stockRequest);
        }

        // POST: StockRequests/Approve/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
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
            await _vendorSync.WriteStatusToVendorCacheAsync(id, "Approuvée");
            return RedirectToAction("Index");
        }

        // POST: StockRequests/Reject/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Reject(int id)
        {
            StockRequest? request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();
            request.Status = "Rejetée";
            await db.SaveChangesAsync();
            await _vendorSync.WriteStatusToVendorCacheAsync(id, "Rejetée");
            return RedirectToAction("Index");
        }

        // GET: StockRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest("L'identifiant de la demande est requis.");
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            StockRequest? request = await db.StockRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            if (!isAdmin && request.RequestedById != currentUserId) return Forbid();
            return View(request);
        }

        // GET: StockRequests/Edit/5 — admin only
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest("L'identifiant de la demande est requis.");
            StockRequest? request = await db.StockRequests.FindAsync(id);
            if (request == null) return NotFound();

            await PopulateBicycleNamesViewBagAsync();
            return View(request);
        }

        // POST: StockRequests/Edit/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BicycleName,Quantity,Notes")] StockRequest stockRequest)
        {
            if (id != stockRequest.Id)
            {
                return BadRequest("L'identifiant de la demande ne correspond pas.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateBicycleNamesViewBagAsync();
                return View(stockRequest);
            }

            StockRequest? requestToUpdate = await db.StockRequests.FindAsync(id);
            if (requestToUpdate == null)
            {
                return NotFound();
            }

            requestToUpdate.BicycleName = stockRequest.BicycleName;
            requestToUpdate.Quantity = stockRequest.Quantity;
            requestToUpdate.Notes = stockRequest.Notes;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await db.StockRequests.AnyAsync(r => r.Id == id);
                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: StockRequests/Delete/5 — admin only
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("L'identifiant de la demande est requis.");
            StockRequest? request = await db.StockRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();

            return View(request);
        }

        // POST: StockRequests/Delete/5 — admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            StockRequest? request = await db.StockRequests.FindAsync(id);
            if (request != null)
            {
                db.StockRequests.Remove(request);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

