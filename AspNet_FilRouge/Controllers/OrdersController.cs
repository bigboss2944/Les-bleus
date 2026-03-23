using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PageSize = 10;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // GET: Orders — paginated view (all authenticated users)
        public async Task<IActionResult> Index(int page = 1)
        {
            var orders = db.Orders.AsQueryable();
            var paginatedList = await PaginatedList<Order>.CreateAsync(orders, page, PageSize);
            return View(paginatedList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Bicycles)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // Create / Edit — admin only
        [Authorize(Roles = "Administrateur")]
        public IActionResult Create()
        {
            ViewBag.Bicycles = db.Bicycles.Where(b => b.Order == null).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(new Order { Date = DateTime.Now });
        }

        // Partial view helper — retourne la liste déroulante des vélos disponibles
        public IActionResult SelectIdCategory()
        {
            var bicycles = db.Bicycles.Where(b => b.Order == null).ToList();
            return PartialView("~/Views/Shared/_listBicycleDropDownList.cshtml", new BicycleOrdersViewModel { Bicycles = bicycles });
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order, long? BicycleId, long? CustomerId, long? ShopId, List<long>? BicycleIds)
        {
            if (ModelState.IsValid)
            {
                // Associer la commande au vendeur connecté
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    order.Seller = await db.Sellers.FirstOrDefaultAsync(s => s.Id == currentUser.Id);
                }

                if (CustomerId.HasValue)
                    order.Customer = await db.Customers.FindAsync(CustomerId.Value);

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

            ViewBag.Bicycles = db.Bicycles.Where(b => b.Order == null).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(order);
        }

        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Bicycles)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();

            ViewBag.Bicycles = db.Bicycles.Where(b => b.Order == null || b.Order.IdOrder == id).ToList();
            ViewBag.Customers = db.Customers.ToList();
            ViewBag.Shops = db.Shops.ToList();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost,IsValidated")] Order order, long? CustomerId, long? ShopId)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.Orders
                    .Include(o => o.Bicycles)
                    .FirstOrDefaultAsync(o => o.IdOrder == order.IdOrder);
                if (existing == null) return NotFound();

                existing.Date = order.Date;
                existing.PayMode = order.PayMode;
                existing.Discount = order.Discount;
                existing.UseLoyaltyPoint = order.UseLoyaltyPoint;
                existing.Tax = order.Tax;
                existing.ShippingCost = order.ShippingCost;
                existing.IsValidated = order.IsValidated;

                if (CustomerId.HasValue)
                    existing.Customer = await db.Customers.FindAsync(CustomerId.Value);

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
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order != null)
            {
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
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles.FindAsync(bicycleId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable." });
            if (bicycle.Order != null && bicycle.Order.IdOrder != orderId)
                return BadRequest(new { error = "Ce vélo est déjà associé à une autre commande." });

            bicycle.Order = order;
            await db.SaveChangesAsync();

            return Ok(new { total = CalculateTotal(order) });
        }

        // POST: Orders/RemoveBicycle — retire un vélo d'une commande existante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles.FirstOrDefaultAsync(b => b.Id == bicycleId && b.Order != null && b.Order.IdOrder == orderId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable dans cette commande." });

            bicycle.Order = null;
            await db.SaveChangesAsync();

            return Ok(new { total = CalculateTotal(order) });
        }

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return Ok(new { total = CalculateTotal(order) });
        }

        // POST: Orders/Validate/5 — valide une commande (requiert connexion)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(long id)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (order.IsValidated) return BadRequest(new { error = "La commande est déjà validée." });
            if (!order.Bicycles.Any()) return BadRequest(new { error = "Impossible de valider une commande sans produits." });

            order.IsValidated = true;
            await db.SaveChangesAsync();

            return Ok(new { message = "Commande validée avec succès.", total = CalculateTotal(order) });
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static float CalculateTotal(Order order)
        {
            float subtotal = order.Bicycles?.Sum(b => b.FreeTaxPrice) ?? 0f;
            float afterDiscount = subtotal * (1 - order.Discount / 100f);
            float withTax = afterDiscount * (1 + order.Tax / 100f);
            return withTax + order.ShippingCost;
        }
    }
}
