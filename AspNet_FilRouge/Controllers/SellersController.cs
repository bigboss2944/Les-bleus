using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class SellersController : Controller
    {
        private readonly ApplicationDbContext db;

        public SellersController(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Sellers.ToListAsync());
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return BadRequest();
            Seller? seller = await db.Sellers.FindAsync(id);
            if (seller == null) return NotFound();
            return View(seller);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstName")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Sellers.Add(seller);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(seller);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return BadRequest();
            Seller? seller = await db.Sellers.FindAsync(id);
            if (seller == null) return NotFound();
            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,LastName,FirstName")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(seller);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return BadRequest();
            Seller? seller = await db.Sellers.FindAsync(id);
            if (seller == null) return NotFound();
            return View(seller);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Seller? seller = await db.Sellers.FindAsync(id);
            if (seller != null)
            {
                db.Sellers.Remove(seller);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
