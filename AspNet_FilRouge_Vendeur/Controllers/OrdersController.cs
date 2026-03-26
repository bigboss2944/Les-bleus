using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using AspNet_FilRouge_Vendeur.Models;

namespace AspNet_FilRouge_Vendeur.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderPricingService _pricingService;
        private const int PageSize = AppConstants.Pagination.DefaultPageSize;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOrderPricingService pricingService)
        {
            db = context;
            _userManager = userManager;
            _pricingService = pricingService;
        }

        // GET: Orders — paginated view with optional seller filter (all authenticated users)
        public async Task<IActionResult> Index(int page = 1, string? sellerId = null)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var orders = db.Orders.Include(o => o.Seller).AsQueryable();

            if (isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(sellerId))
                    orders = orders.Where(o => o.Seller != null && o.Seller.Id == sellerId);
            }
            else
            {
                orders = orders.Where(o => o.Seller != null && o.Seller.Id == currentUserId);
                sellerId = currentUserId;
            }

            orders = orders
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.IdOrder);

            var paginatedList = await PaginatedList<Order>.CreateAsync(orders, page, PageSize);

            ViewBag.Sellers = await db.Sellers
                .Where(s => isAdmin || s.Id == currentUserId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
            ViewBag.CurrentSellerId = sellerId;
            ViewBag.CurrentUserId = currentUserId;

            return View(paginatedList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return BadRequest();
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Bicycles)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();
            return View(order);
        }

        // Create — vendeurs et administrateurs
        [Authorize(Roles = "Administrateur,Vendeur")]
        public IActionResult Create()
        {
            ViewBag.Bicycles = db.Bicycles.Where(b => b.Quantity > 0).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(new Order { Date = DateTime.Now });
        }

        // Partial view helper — retourne la liste déroulante des vélos disponibles
        public IActionResult SelectIdCategory()
        {
            var bicycles = db.Bicycles.Where(b => b.Quantity > 0).ToList();
            return PartialView("~/Views/Shared/_listBicycleDropDownList.cshtml", new BicycleOrdersViewModel { Bicycles = bicycles });
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Create([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order, long? BicycleId, string? CustomerId, long? ShopId, List<long>? BicycleIds)
        {
            if (ModelState.IsValid)
            {
                // Associer la commande au vendeur connecté
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == currentUser.Id);
                    if (seller == null)
                    {
                        seller = new Seller
                        {
                            Id = currentUser.Id,
                            UserName = currentUser.UserName,
                            Email = currentUser.Email,
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            PhoneNumber = currentUser.PhoneNumber
                        };
                        db.Sellers.Add(seller);
                    }

                    order.Seller = seller;
                }

                if (!string.IsNullOrWhiteSpace(CustomerId))
                    order.Customer = await db.Customers.FindAsync(CustomerId);

                if (ShopId.HasValue)
                    order.Shop = await db.Shops.FindAsync(ShopId.Value);

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                // Associer les vélos sélectionnés
                if (BicycleIds != null && BicycleIds.Count > 0)
                {
                    var bicycles = await db.Bicycles
                        .Where(b => BicycleIds.Contains(b.Id))
                        .ToListAsync();
                    foreach (var bicycle in bicycles)
                        bicycle.Order = order;
                    await db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bicycles = db.Bicycles.Where(b => b.Quantity > 0).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(order);
        }

        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();

            // Vendors can only edit their own orders
            if (!User.IsInRole(AppConstants.Roles.Administrateur))
            {
                var currentUserId = _userManager.GetUserId(User);
                if (order.Seller?.Id != currentUserId)
                    return Forbid();

                if (order.IsValidated)
                    return Forbid();
            }

            ViewBag.Bicycles = db.Bicycles.Where(b => b.Order == null || b.Order.IdOrder == id).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Edit([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost,IsValidated")] Order order, string? CustomerId, long? ShopId)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.Orders
                    .Include(o => o.Bicycles)
                    .Include(o => o.Seller)
                    .FirstOrDefaultAsync(o => o.IdOrder == order.IdOrder);
                if (existing == null) return NotFound();

                // Vendors can only edit their own orders
                if (!User.IsInRole(AppConstants.Roles.Administrateur))
                {
                    var currentUserId = _userManager.GetUserId(User);
                    if (existing.Seller?.Id != currentUserId)
                        return Forbid();

                    if (existing.IsValidated)
                        return Forbid();
                }

                existing.Date = order.Date;
                existing.PayMode = order.PayMode;
                existing.Discount = order.Discount;
                existing.UseLoyaltyPoint = order.UseLoyaltyPoint;
                existing.Tax = order.Tax;
                existing.ShippingCost = order.ShippingCost;
                if (User.IsInRole(AppConstants.Roles.Administrateur))
                    existing.IsValidated = order.IsValidated;

                if (!string.IsNullOrWhiteSpace(CustomerId))
                    existing.Customer = await db.Customers.FindAsync(CustomerId);

                if (ShopId.HasValue)
                    existing.Shop = await db.Shops.FindAsync(ShopId.Value);

                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bicycles = db.Bicycles.Where(b => b.Order == null || b.Order.IdOrder == order.IdOrder).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(order);
        }

        // Cancel — admin only (shown as Delete in existing flow)
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();

            if (!User.IsInRole(AppConstants.Roles.Administrateur))
            {
                var currentUserId = _userManager.GetUserId(User);
                if (order.Seller?.Id != currentUserId)
                    return Forbid();

                if (order.IsValidated)
                    return Forbid();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order != null)
            {
                if (!User.IsInRole(AppConstants.Roles.Administrateur))
                {
                    var currentUserId = _userManager.GetUserId(User);
                    if (order.Seller?.Id != currentUserId)
                        return Forbid();

                    if (order.IsValidated)
                        return Forbid();
                }

                // Détacher les vélos avant de supprimer la commande
                foreach (var b in order.Bicycles)
                    b.Order = null;
                db.Orders.Remove(order);
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ── AJAX actions ──────────────────────────────────────────────────────

        // POST: Orders/AddBicycle — ajoute un vélo à une commande existante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBicycle(long orderId, long bicycleId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Vous ne pouvez modifier que vos propres commandes." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles.FindAsync(bicycleId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable." });
            if (bicycle.Quantity <= 0) return BadRequest(new { error = "Stock insuffisant pour ce vélo." });
            if (bicycle.Order != null && bicycle.Order.IdOrder != orderId)
                return BadRequest(new { error = "Ce vélo est déjà associé à une autre commande." });

            bicycle.Order = order;
            await db.SaveChangesAsync();

            return Ok(new { total = _pricingService.CalculateTotal(order) });
        }

        // POST: Orders/RemoveBicycle — retire un vélo d'une commande existante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Vous ne pouvez modifier que vos propres commandes." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles.FirstOrDefaultAsync(b => b.Id == bicycleId && b.Order != null && b.Order.IdOrder == orderId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable dans cette commande." });

            bicycle.Order = null;
            await db.SaveChangesAsync();

            return Ok(new { total = _pricingService.CalculateTotal(order) });
        }

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();
            return Ok(new { total = _pricingService.CalculateTotal(order) });
        }

        // POST: Orders/Validate/5 — valide une commande (requiert connexion)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(long id)
        {
            if (User.IsInRole(AppConstants.Roles.Vendeur) && !User.IsInRole(AppConstants.Roles.Administrateur))
                return Forbid();

            if (IsClientOfflineRequest())
                return BadRequest(new { error = "Validation impossible en mode hors-ligne." });

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();
            if (order.IsValidated) return BadRequest(new { error = "La commande est déjà validée." });
            if (!order.Bicycles.Any()) return BadRequest(new { error = "Impossible de valider une commande sans produits." });

            var outOfStock = order.Bicycles.FirstOrDefault(b => b.Quantity <= 0);
            if (outOfStock != null)
                return BadRequest(new { error = $"Stock insuffisant pour le vélo #{outOfStock.Id}." });

            foreach (var bicycle in order.Bicycles)
            {
                bicycle.Quantity -= 1;
            }

            order.IsValidated = true;
            await db.SaveChangesAsync();

            return Ok(new { message = "Commande validée avec succès.", total = _pricingService.CalculateTotal(order) });
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private bool IsClientOfflineRequest()
        {
            if (!Request.Headers.TryGetValue("X-Client-Online", out StringValues headerValues))
                return false;

            var value = headerValues.ToString();
            return value.Equals("false", StringComparison.OrdinalIgnoreCase)
                || value.Equals("0", StringComparison.OrdinalIgnoreCase)
                || value.Equals("offline", StringComparison.OrdinalIgnoreCase);
        }
    }
}
