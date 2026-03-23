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

        // GET: Orders — paginated view (all authenticated users)
        public async Task<IActionResult> Index(int page = 1)
        {
            var orders = db.Orders.AsQueryable();
            var paginatedList = await PaginatedList<Order>.CreateAsync(orders, page, PageSize);
            return View(paginatedList);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return BadRequest();
            Order? order = await db.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // Create / Edit — admin only
        [Authorize(Roles = "Administrateur")]
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult SelectIdCategory()
        {
            List<Bicycle> bicycles = db.Bicycles.ToList();
            BicycleOrdersViewModel viewModel = new BicycleOrdersViewModel
            {
                Bicycles = bicycles
            };
            return PartialView("~/Views/Shared/_listBicycleDropDownList.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Create([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order, long? BicycleId)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return BadRequest();
            Order? order = await db.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Edit([Bind("IdOrder,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // Cancel — admin only (shown as Delete in existing flow)
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return BadRequest();
            Order? order = await db.Orders.FindAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrateur")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            Order? order = await db.Orders.FindAsync(id);
            if (order != null)
            {
                db.Orders.Remove(order);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
