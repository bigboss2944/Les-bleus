using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext db;

        public CustomersController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await db.Customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return BadRequest();
            Customer? customer = await db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Town,PostalCode,Address,LoyaltyPoints,Phone,Email,Gender,LastName,FirstName")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return BadRequest();
            Customer? customer = await db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Town,PostalCode,Address,LoyaltyPoints,Phone,Email,Gender,LastName,FirstName")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return BadRequest();
            Customer? customer = await db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Customer? customer = await db.Customers.FindAsync(id);
            if (customer != null)
            {
                db.Customers.Remove(customer);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
