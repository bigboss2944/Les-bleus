using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private const int PageSize = AppConstants.Pagination.DefaultPageSize;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Orders — paginated view (all authenticated users see all orders)
        public async Task<IActionResult> Index(int page = 1)
        {
            var paginatedList = await _orderService.GetPagedAsync(page, PageSize, sellerId: null);
            return View(paginatedList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return BadRequest();

            var order = await _orderService.GetDetailsByIdAsync(id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: Orders/Cancel/5 — confirmation d'annulation (admin uniquement)
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Cancel(long? id)
        {
            if (id == null) return BadRequest();
            var order = await _orderService.GetDetailsByIdAsync(id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Cancel/5 — annule (supprime) la commande (admin uniquement)
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> CancelConfirmed(long id)
        {
            await _orderService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        // GET: Orders/GetPrice/5 — calcule le prix total d'une commande
        [HttpGet]
        public async Task<IActionResult> GetPrice(long id)
        {
            var order = await _orderService.GetWithBicyclesAndSellerAsync(id);
            if (order == null) return NotFound();
            return Ok(new { total = _orderService.CalculateTotal(order) });
        }

        // POST: Orders/AddBicycle — ajoute un vélo à une commande existante (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> AddBicycle(long orderId, long bicycleId)
        {
            var result = await _orderService.AddBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // POST: Orders/RemoveBicycle — retire un vélo d'une commande existante (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var result = await _orderService.RemoveBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // POST: Orders/Validate/5 — valide une commande (admin uniquement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstants.Roles.Administrateur)]
        public async Task<IActionResult> Validate(long id)
        {
            var result = await _orderService.ValidateAsync(id);
            if (result.Status == OrderOperationStatus.Success)
            {
                return Ok(new { message = result.Message, total = result.Total });
            }
            return ToActionResult(result);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

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
