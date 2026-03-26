using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private const int PageSize = 10;

        public OrdersController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Orders — paginated view (all authenticated users see all orders)
        public async Task<IActionResult> Index(int page = 1)
        {
            var orders = db.Orders
                .Include(o => o.Seller)
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.IdOrder)
                .AsQueryable();
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

        // GET: Orders/Cancel/5 — confirmation d'annulation (admin uniquement)
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Cancel(long? id)
        {
            if (id == null) return BadRequest();
            var order = await db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Bicycles)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Cancel/5 — annule (supprime) la commande (admin uniquement)
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> CancelConfirmed(long id)
        {
            var order = await db.Orders.FindAsync(id);
            if (order != null)
            {
                db.Orders.Remove(order);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var order = await db.Orders
                .Include(o => o.Bicycles)
                .FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return Ok(new { total = CalculateTotal(order) });
        }

        // POST: Orders/AddBicycle — ajoute un vélo à une commande existante (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> AddBicycle(long orderId, long bicycleId)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles.Include(b => b.Order).FirstOrDefaultAsync(b => b.Id == bicycleId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable." });
            if (bicycle.Quantity <= 0) return BadRequest(new { error = "Stock insuffisant pour ce vélo." });
            if (bicycle.Order != null && bicycle.Order.IdOrder != orderId)
                return BadRequest(new { error = "Ce vélo est déjà associé à une autre commande." });

            bicycle.Order = order;
            await db.SaveChangesAsync();

            return Ok(new { total = CalculateTotal(order) });
        }

        // POST: Orders/RemoveBicycle — retire un vélo d'une commande existante (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == orderId);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            if (order.IsValidated) return BadRequest(new { error = "Impossible de modifier une commande validée." });

            var bicycle = await db.Bicycles
                .Include(b => b.Order)
                .FirstOrDefaultAsync(b => b.Id == bicycleId && b.Order != null && b.Order.IdOrder == orderId);
            if (bicycle == null) return NotFound(new { error = "Vélo introuvable dans cette commande." });

            bicycle.Order = null;
            await db.SaveChangesAsync();

            return Ok(new { total = CalculateTotal(order) });
        }

        // POST: Orders/Validate/5 — valide une commande (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Validate(long id)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
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
