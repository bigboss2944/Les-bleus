using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class BicyclesController : Controller
    {
        private readonly ApplicationDbContext db;
        private const int PageSize = AppConstants.Pagination.DefaultPageSize;

        public BicyclesController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Bicycles — paginated catalog view
        public async Task<IActionResult> Index(int page = 1)
        {
            var bicycles = db.Bicycles
                .OrderBy(b => b.Id)
                .AsQueryable();
            var paginatedList = await PaginatedList<Bicycle>.CreateAsync(bicycles, page, PageSize);

            ViewBag.StockSummaries = await db.Bicycles
                .GroupBy(b => new { b.TypeOfBike, b.Reference, b.Color })
                .Select(group => new StockSummaryViewModel
                {
                    TypeOfBike = group.Key.TypeOfBike,
                    Reference = group.Key.Reference,
                    Color = group.Key.Color,
                    Quantity = group.Sum(b => b.Quantity)
                })
                .OrderBy(summary => summary.TypeOfBike)
                .ThenBy(summary => summary.Reference)
                .ThenBy(summary => summary.Color)
                .ToListAsync();

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

            db.Bicycles.Add(bicycle);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Bicycles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
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

            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
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
                db.Update(bicycle);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await db.Bicycles.AnyAsync(b => b.Id == bicycle.Id);
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

            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
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
            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return NotFound();
            }

            db.Bicycles.Remove(bicycle);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
