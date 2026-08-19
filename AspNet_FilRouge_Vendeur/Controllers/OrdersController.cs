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
        // Shops are out of the Repository/Service scope for this refactor; kept as direct DbContext access.
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly IBicycleService _bicycleService;
        private readonly ICustomerService _customerService;
        private readonly ISellerService _sellerService;
        private const int PageSize = AppConstants.Pagination.DefaultPageSize;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IOrderService orderService,
            IBicycleService bicycleService,
            ICustomerService customerService,
            ISellerService sellerService)
        {
            db = context;
            _userManager = userManager;
            _orderService = orderService;
            _bicycleService = bicycleService;
            _customerService = customerService;
            _sellerService = sellerService;
        }

        // GET: Orders — paginated view with optional seller filter (all authenticated users)
        public async Task<IActionResult> Index(int page = 1, string? sellerId = null)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var effectiveSellerId = isAdmin ? sellerId : currentUserId;

            var paginatedList = await _orderService.GetPagedAsync(page, PageSize, effectiveSellerId);

            ViewBag.Sellers = await _sellerService.GetFilteredAsync(isAdmin, currentUserId);
            ViewBag.CurrentSellerId = effectiveSellerId;
            ViewBag.CurrentUserId = currentUserId;

            return View(paginatedList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return BadRequest();
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await _orderService.GetDetailsByIdAsync(id.Value);
            if (order == null) return NotFound();
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();
            return View(order);
        }

        // Create — vendeurs et administrateurs
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Bicycles = await _bicycleService.GetAvailableAsync();
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Shops = await db.Shops.ToListAsync();
            return View(new Order { Date = DateTime.Now });
        }

        // Partial view helper — retourne la liste déroulante des vélos disponibles
        public async Task<IActionResult> SelectIdCategory()
        {
            var bicycles = await _bicycleService.GetAvailableAsync();
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
                    var seller = await _sellerService.GetByIdAsync(currentUser.Id);
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
                        await _sellerService.CreateAsync(seller);
                    }

                    order.Seller = seller;
                }

                if (!string.IsNullOrWhiteSpace(CustomerId))
                    order.Customer = await _customerService.GetByIdAsync(CustomerId);

                if (ShopId.HasValue)
                    order.Shop = await db.Shops.FindAsync(ShopId.Value);

                await _orderService.CreateAsync(order);

                // Associer les vélos sélectionnés
                if (BicycleIds != null && BicycleIds.Count > 0)
                {
                    var bicycles = await _bicycleService.GetByIdsAsync(BicycleIds);
                    foreach (var bicycle in bicycles)
                        bicycle.Order = order;
                    await _orderService.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bicycles = await _bicycleService.GetAvailableAsync();
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Shops = await db.Shops.ToListAsync();
            return View(order);
        }

        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return BadRequest();
            var order = await _orderService.GetDetailsByIdAsync(id.Value);
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

            ViewBag.Bicycles = await _bicycleService.GetAvailableForOrderAsync(id.Value);
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Shops = await db.Shops.ToListAsync();
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
                var existing = await _orderService.GetWithBicyclesAndSellerAsync(order.IdOrder);
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
                    existing.Customer = await _customerService.GetByIdAsync(CustomerId);

                if (ShopId.HasValue)
                    existing.Shop = await db.Shops.FindAsync(ShopId.Value);

                await _orderService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bicycles = await _bicycleService.GetAvailableForOrderAsync(order.IdOrder);
            ViewBag.Customers = await _customerService.GetAllAsync();
            ViewBag.Shops = await db.Shops.ToListAsync();
            return View(order);
        }

        // Cancel — admin only (shown as Delete in existing flow)
        [Authorize(Roles = "Administrateur,Vendeur")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return BadRequest();
            var order = await _orderService.GetDetailsByIdAsync(id.Value);
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
            var order = await _orderService.GetWithBicyclesAndSellerAsync(id);
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

                await _orderService.DeleteAsync(id);
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

            var order = await _orderService.GetWithBicyclesAndSellerAsync(orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Vous ne pouvez modifier que vos propres commandes." });

            var result = await _orderService.AddBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // POST: Orders/RemoveBicycle — retire un vélo d'une commande existante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await _orderService.GetWithBicyclesAndSellerAsync(orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "Vous ne pouvez modifier que vos propres commandes." });

            var result = await _orderService.RemoveBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole(AppConstants.Roles.Administrateur);

            var order = await _orderService.GetWithBicyclesAndSellerAsync(id);
            if (order == null) return NotFound();
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();
            return Ok(new { total = _orderService.CalculateTotal(order) });
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

            var order = await _orderService.GetWithBicyclesAndSellerAsync(id);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (!isAdmin && order.Seller?.Id != currentUserId) return Forbid();

            var result = await _orderService.ValidateAsync(id);
            if (result.Status == OrderOperationStatus.Success)
            {
                return Ok(new { message = result.Message, total = result.Total });
            }
            return ToActionResult(result);
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

        private IActionResult ToActionResult(OrderOperationResult result)
        {
            return result.Status switch
            {
                OrderOperationStatus.NotFound => NotFound(new { error = result.Error }),
                OrderOperationStatus.InvalidState => BadRequest(new { error = result.Error }),
                _ => Ok(new { total = result.Total })
            };
        }
    }
}
