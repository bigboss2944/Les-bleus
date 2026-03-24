using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge_Vendeur.Models;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize(Roles = "Administrateur,Vendeur")]
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

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Town,PostalCode,Address,LoyaltyPoints,Phone,Email,Gender,LastName,FirstName")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return BadRequest();
            Customer? customer = await db.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Town,PostalCode,Address,LoyaltyPoints,Phone,Email,Gender,LastName,FirstName")] Customer customer)
        {
            if (id != customer.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await db.Customers.FindAsync(id);
                if (existing == null) return NotFound();

                db.Entry(existing).CurrentValues.SetValues(customer);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
    }
}
