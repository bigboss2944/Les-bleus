using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge_Vendeur.Models;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize(Roles = "Administrateur")]
    public class SellersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
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

        // GET: Sellers/Create — create a new vendor account
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sellers/Create — create ApplicationUser + Seller + assign Vendeur role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSellerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Address = model.Address,
                    PhoneNumber = model.Phone,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Vendeur");

                    db.Sellers.Add(new Seller
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber
                    });
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
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
