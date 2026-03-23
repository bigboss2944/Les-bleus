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

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var order = await db.Orders.Include(o => o.Bicycles).FirstOrDefaultAsync(o => o.IdOrder == id);
            if (order == null) return NotFound();
            return Ok(new { total = CalculateTotal(order) });
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
