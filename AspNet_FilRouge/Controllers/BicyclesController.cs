using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
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

        // GET: Bicycles — paginated catalog view
        public async Task<IActionResult> Index(int page = 1)
        {
            var paginatedList = await _bicycleService.GetPagedAsync(page, PageSize);
            ViewBag.StockSummaries = await _bicycleService.GetStockSummariesAsync();
            return View(paginatedList);
        }

        // GET: Bicycles/Create
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bicycles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Create([Bind("TypeOfBike,Category,Reference,FreeTaxPrice,Quantity,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (!ModelState.IsValid)
            {
                return View(bicycle);
            }

            await _bicycleService.CreateAsync(bicycle);
            return RedirectToAction(nameof(Index));
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

        // GET: Bicycles/Edit/5
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

        // POST: Bicycles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TypeOfBike,Category,Reference,FreeTaxPrice,Quantity,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (id != bicycle.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(bicycle);
            }

            try
            {
                await _bicycleService.UpdateAsync(bicycle);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _bicycleService.ExistsAsync(bicycle.Id);
                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bicycles/Delete/5
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

        // POST: Bicycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var deleted = await _bicycleService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
