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
            var requests = await db.StockRequests
                .Include(r => r.RequestedBy)
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
            request.Status = "Approuvée";
            await db.SaveChangesAsync();
            await _vendorSync.WriteStatusToVendorCacheAsync(id, "Approuvée");
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
            await _vendorSync.WriteStatusToVendorCacheAsync(id, "Rejetée");
            return RedirectToAction("Index");
        }

        // GET: StockRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest("L'identifiant de la demande est requis.");
            StockRequest? request = await db.StockRequests
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();
            return View(request);
        }
    }
}

