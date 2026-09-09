using AspNet_FilRouge.Models.Api;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNet_FilRouge.Controllers.Api
{
    /// <summary>
    /// API REST de gestion des commandes pour l'application MAUI.
    /// Toutes les opérations d'écriture nécessitent une connexion (l'appel HTTP échoue
    /// simplement si le mobile est hors-ligne ; c'est le client qui bloque la validation
    /// hors-ligne, conformément au cahier des charges).
    /// </summary>
    [ApiController]
    [Route("api/mobile/orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = AppConstants.Roles.Vendeur + "," + AppConstants.Roles.Administrateur)]
    public class MobileOrdersController : ControllerBase
    {
        private const int MaxPageSize = 50;

        private readonly IOrderService _orderService;
        private readonly ISellerService _sellerService;
        private readonly ICustomerService _customerService;
        private readonly IOrderPricingService _pricingService;

        public MobileOrdersController(
            IOrderService orderService,
            ISellerService sellerService,
            ICustomerService customerService,
            IOrderPricingService pricingService)
        {
            _orderService = orderService;
            _sellerService = sellerService;
            _customerService = customerService;
            _pricingService = pricingService;
        }

        // GET api/mobile/orders?page=1&pageSize=20 — commandes de tous les vendeurs, paginées
        [HttpGet]
        public async Task<IActionResult> GetOrders(int page = 1, int pageSize = AppConstants.Pagination.DefaultPageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > MaxPageSize) pageSize = AppConstants.Pagination.DefaultPageSize;

            var paged = await _orderService.GetPagedAsync(page, pageSize, sellerId: null);

            var result = new MobilePagedResult<MobileOrderSummary>
            {
                Page = paged.PageIndex,
                PageSize = pageSize,
                TotalPages = paged.TotalPages,
                HasNextPage = paged.HasNextPage,
                Items = paged.Select(ToSummary).ToList()
            };

            return Ok(result);
        }

        // GET api/mobile/orders/5
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetOrder(long id)
        {
            var order = await _orderService.GetDetailsByIdAsync(id);
            if (order == null) return NotFound(new { error = "Commande introuvable." });
            return Ok(ToDetail(order));
        }

        // POST api/mobile/orders — crée une nouvelle commande (vide, sans vélo) pour le vendeur connecté
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] MobileCreateOrderRequest request)
        {
            var sellerId = request.SellerId ?? CurrentUserId;
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest(new { error = "Vendeur non identifié." });

            var seller = await _sellerService.GetByIdAsync(sellerId);
            if (seller == null)
                return BadRequest(new { error = "Vendeur introuvable." });

            Customer? customer = null;
            if (!string.IsNullOrWhiteSpace(request.CustomerId))
            {
                customer = await _customerService.GetByIdAsync(request.CustomerId);
                if (customer == null)
                    return BadRequest(new { error = "Client introuvable." });
            }

            var order = new Order
            {
                Date = DateTime.UtcNow,
                Seller = seller,
                Customer = customer,
                Discount = Math.Clamp(request.Discount, 0f, 100f),
                PayMode = request.PayMode
            };

            await _orderService.CreateAsync(order);

            var created = await _orderService.GetDetailsByIdAsync(order.IdOrder);
            return CreatedAtAction(nameof(GetOrder), new { id = order.IdOrder }, ToDetail(created!));
        }

        // POST api/mobile/orders/5/bicycles/12 — ajoute un vélo à la commande, retourne le total à jour
        [HttpPost("{orderId:long}/bicycles/{bicycleId:long}")]
        public async Task<IActionResult> AddBicycle(long orderId, long bicycleId)
        {
            var result = await _orderService.AddBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // DELETE api/mobile/orders/5/bicycles/12 — retire un vélo de la commande, retourne le total à jour
        [HttpDelete("{orderId:long}/bicycles/{bicycleId:long}")]
        public async Task<IActionResult> RemoveBicycle(long orderId, long bicycleId)
        {
            var result = await _orderService.RemoveBicycleAsync(orderId, bicycleId);
            return ToActionResult(result);
        }

        // POST api/mobile/orders/5/validate — valide la commande (nécessite une connexion, donc cet appel doit réussir)
        [HttpPost("{id:long}/validate")]
        public async Task<IActionResult> Validate(long id)
        {
            var result = await _orderService.ValidateAsync(id);
            return ToActionResult(result);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private string? CurrentUserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        private IActionResult ToActionResult(OrderOperationResult result)
        {
            return result.Status switch
            {
                OrderOperationStatus.NotFound => NotFound(new { error = result.Error }),
                OrderOperationStatus.InvalidState => BadRequest(new { error = result.Error }),
                _ => Ok(new { message = result.Message, total = result.Total })
            };
        }

        private MobileOrderSummary ToSummary(Order order) => new()
        {
            IdOrder = order.IdOrder,
            Date = order.Date,
            IsValidated = order.IsValidated,
            Discount = order.Discount,
            Total = _pricingService.CalculateTotal(order),
            BicycleCount = order.Bicycles.Count,
            Seller = order.Seller == null ? null : new MobileSellerSummary
            {
                Id = order.Seller.Id,
                FirstName = order.Seller.FirstName,
                LastName = order.Seller.LastName
            },
            Customer = order.Customer == null ? null : new MobileCustomerSummary
            {
                Id = order.Customer.Id,
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                Email = order.Customer.Email
            }
        };

        private MobileOrderDetail ToDetail(Order order) => new()
        {
            IdOrder = order.IdOrder,
            Date = order.Date,
            IsValidated = order.IsValidated,
            Discount = order.Discount,
            Tax = order.Tax,
            ShippingCost = order.ShippingCost,
            Total = _pricingService.CalculateTotal(order),
            Seller = order.Seller == null ? null : new MobileSellerSummary
            {
                Id = order.Seller.Id,
                FirstName = order.Seller.FirstName,
                LastName = order.Seller.LastName
            },
            Customer = order.Customer == null ? null : new MobileCustomerSummary
            {
                Id = order.Customer.Id,
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                Email = order.Customer.Email
            },
            Bicycles = order.Bicycles.Select(b => new MobileBicycleSummary
            {
                Id = b.Id,
                TypeOfBike = b.TypeOfBike,
                Category = b.Category,
                Reference = b.Reference,
                FreeTaxPrice = b.FreeTaxPrice,
                Tax = b.Tax,
                Quantity = b.Quantity,
                Exchangeable = b.Exchangeable,
                Insurance = b.Insurance,
                Deliverable = b.Deliverable
            }).ToList()
        };
    }
}
