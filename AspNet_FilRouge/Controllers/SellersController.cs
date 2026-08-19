using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspNet_FilRouge.Controllers
{
    [Authorize(Roles = AppConstants.Roles.Administrateur)]
    public class SellersController : Controller
    {
        // Shops are out of the Repository/Service scope for this refactor; kept as direct DbContext access.
        private readonly ApplicationDbContext db;
        private readonly ISellerService _sellerService;

        public SellersController(ApplicationDbContext context, ISellerService sellerService)
        {
            db = context;
            _sellerService = sellerService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _sellerService.GetAllAsync());
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return BadRequest();
            Seller? seller = await _sellerService.GetByIdAsync(id);
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
                var result = await _sellerService.CreateSellerAccountAsync(model);
                if (result.Succeeded)
                {
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
            Seller? seller = await _sellerService.GetByIdWithShopAsync(id);
            if (seller == null) return NotFound();
            ViewBag.ShopId = new SelectList(await db.Shops.ToListAsync(), "ShopId", "Name", seller.Shop?.ShopId);
            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string? lastName, string? firstName, string? email, string? phoneNumber, long? shopId)
        {
            Seller? seller = await _sellerService.GetByIdWithShopAsync(id);
            if (seller == null) return NotFound();

            seller.LastName = lastName;
            seller.FirstName = firstName;
            seller.Email = email;
            seller.PhoneNumber = phoneNumber;
            seller.Shop = shopId.HasValue ? await db.Shops.FindAsync(shopId.Value) : null;

            if (ModelState.IsValid)
            {
                await _sellerService.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ShopId = new SelectList(await db.Shops.ToListAsync(), "ShopId", "Name", shopId);
            return View(seller);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return BadRequest();
            Seller? seller = await _sellerService.GetByIdAsync(id);
            if (seller == null) return NotFound();
            return View(seller);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _sellerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
