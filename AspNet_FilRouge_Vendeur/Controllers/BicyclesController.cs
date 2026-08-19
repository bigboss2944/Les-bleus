using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet_FilRouge_Vendeur.Models;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize]
    public class BicyclesController : Controller
    {
        private readonly IBicycleService _bicycleService;
        private const int PageSize = AppConstants.Pagination.DefaultPageSize;

        public BicyclesController(IBicycleService bicycleService)
        {
            _bicycleService = bicycleService;
        }

        // GET: Bicycles — paginated stock view
        public async Task<IActionResult> Index(int page = 1)
        {
            var paginatedList = await _bicycleService.GetPagedAsync(page, PageSize);
            ViewBag.StockSummaries = await _bicycleService.GetStockSummariesAsync();
            return View(paginatedList);
        }

        // GET: Bicycles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bicycle? bicycle = await _bicycleService.GetByIdAsync(id.Value);
            if (bicycle == null)
            {
                return NotFound();
            }
            return View(bicycle);
        }

        // GET: Bicycles/Create — admin only
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bicycles/Create — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Create([Bind("Id,TypeOfBike,Category,Reference,FreeTaxPrice,Quantity,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                await _bicycleService.CreateAsync(bicycle);
                return RedirectToAction("Index");
            }
            return View(bicycle);
        }

        // GET: Bicycles/Edit/5 — admin only
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bicycle? bicycle = await _bicycleService.GetByIdAsync(id.Value);
            if (bicycle == null)
            {
                return NotFound();
            }
            return View(bicycle);
        }

        // POST: Bicycles/Edit/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Edit([Bind("Id,TypeOfBike,Category,Reference,FreeTaxPrice,Quantity,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                await _bicycleService.UpdateAsync(bicycle);
                return RedirectToAction("Index");
            }
            return View(bicycle);
        }

        // GET: Bicycles/Delete/5 — admin only
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bicycle? bicycle = await _bicycleService.GetByIdAsync(id.Value);
            if (bicycle == null)
            {
                return NotFound();
            }
            return View(bicycle);
        }

        // POST: Bicycles/Delete/5 — admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _bicycleService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
