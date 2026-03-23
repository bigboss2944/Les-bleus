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
        private const int PageSize = 10;

        public BicyclesController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Bicycles — paginated stock view
        public async Task<IActionResult> Index(int page = 1)
        {
            var bicycles = db.Bicycles.AsQueryable();
            var paginatedList = await PaginatedList<Bicycle>.CreateAsync(bicycles, page, PageSize);
            return View(paginatedList);
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

        // GET: Bicycles/Create — admin only
        [Authorize(Roles = "Administrateur")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bicycles/Create — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("Id,TypeOfBike,Category,Reference,FreeTaxPrice,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                db.Bicycles.Add(bicycle);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bicycle);
        }

        // GET: Bicycles/Edit/5 — admin only
        [Authorize(Roles = "Administrateur")]
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

        // POST: Bicycles/Edit/5 — admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit([Bind("Id,TypeOfBike,Category,Reference,FreeTaxPrice,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bicycle).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bicycle);
        }

        // GET: Bicycles/Delete/5 — admin only
        [Authorize(Roles = "Administrateur")]
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

        // POST: Bicycles/Delete/5 — admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle != null)
            {
                db.Bicycles.Remove(bicycle);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
