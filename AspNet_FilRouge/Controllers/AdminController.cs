using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize(Roles = "Administrateur")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: Admin/CreateSeller
        public IActionResult CreateSeller()
        {
            return View();
        }

        // POST: Admin/CreateSeller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSeller(CreateSellerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Address = model.Address,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Vendeur");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            _db.Sellers.Add(new Seller
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Le compte vendeur de {model.FirstName} {model.LastName} a été créé avec succès.";
            return RedirectToAction("CreateSeller");
        }
    }
}
