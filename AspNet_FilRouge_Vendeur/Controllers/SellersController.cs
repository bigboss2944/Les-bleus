using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize(Roles = AppConstants.Roles.Administrateur)]
    public class SellersController : Controller
    {
        private readonly ISellerService _sellerService;

        public SellersController(ISellerService sellerService)
        {
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
            Seller? seller = await _sellerService.GetByIdAsync(id);
            if (seller == null) return NotFound();
            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string? lastName, string? firstName)
        {
            Seller? seller = await _sellerService.GetByIdAsync(id);
            if (seller == null) return NotFound();

            seller.LastName = lastName;
            seller.FirstName = firstName;

            if (ModelState.IsValid)
            {
                await _sellerService.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
