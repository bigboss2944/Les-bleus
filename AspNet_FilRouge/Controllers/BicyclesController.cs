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

        public BicyclesController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET:
        public async Task<IActionResult> Index()
        {
            return View(await db.Bicycles.ToListAsync());
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

        // GET: Bicycles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bicycles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Bicycles/Edit/5
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

        // GET: Bicycles/Delete/5
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
