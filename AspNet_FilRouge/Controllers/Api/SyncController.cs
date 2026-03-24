using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;
using AspNet_FilRouge.Services;

namespace AspNet_FilRouge.Controllers.Api
{
    /// <summary>
    /// API REST de synchronisation — permet de synchroniser
    /// la base locale SQLite avec la base centrale.
    /// </summary>
    [ApiController]
    [Route("api/sync")]
    [Authorize]
    public class SyncController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly LocalDbService _localDb;

        public SyncController(ApplicationDbContext db, LocalDbService localDb)
        {
            _db = db;
            _localDb = localDb;
        }

        // GET api/sync/orders — retourne toutes les commandes
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Bicycles)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            var result = orders.Select(o => new
            {
                o.IdOrder,
                Date = o.Date.ToString("o"),
                o.PayMode,
                o.Discount,
                o.UseLoyaltyPoint,
                o.Tax,
                o.ShippingCost,
                o.IsValidated,
                Seller = o.Seller == null ? null : new { o.Seller.Id, o.Seller.FirstName, o.Seller.LastName },
                Customer = o.Customer == null ? null : new { o.Customer.Id, o.Customer.FirstName, o.Customer.LastName },
                Shop = o.Shop == null ? null : new { o.Shop.ShopId, o.Shop.Nameshop },
                Bicycles = o.Bicycles.Select(b => new
                {
                    b.Id,
                    b.TypeOfBike,
                    b.Category,
                    b.Reference,
                    b.FreeTaxPrice,
                    b.Brand
                })
            });

            return Ok(result);
        }

        // GET api/sync/bicycles — retourne tous les vélos
        [HttpGet("bicycles")]
        public async Task<IActionResult> GetBicycles()
        {
            var bicycles = await _db.Bicycles
                .Include(b => b.Shop)
                .ToListAsync();

            var result = bicycles.Select(b => new
            {
                b.Id,
                b.TypeOfBike,
                b.Category,
                b.Reference,
                b.FreeTaxPrice,
                b.Exchangeable,
                b.Insurance,
                b.Deliverable,
                b.Size,
                b.Weight,
                b.Color,
                b.WheelSize,
                b.Electric,
                b.State,
                b.Brand,
                b.Confort,
                OrderId = b.Order?.IdOrder,
                ShopId = b.Shop?.ShopId
            });

            return Ok(result);
        }

        // GET api/sync/sellers — retourne tous les vendeurs
        [HttpGet("sellers")]
        public async Task<IActionResult> GetSellers()
        {
            var sellers = await _db.Sellers.ToListAsync();
            var result = sellers.Select(s => new
            {
                s.Id,
                s.FirstName,
                s.LastName,
                s.Email
            });
            return Ok(result);
        }

        // GET api/sync/customers — retourne tous les clients
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _db.Customers.ToListAsync();
            var result = customers.Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Town,
                c.PostalCode,
                c.Address,
                c.LoyaltyPoints,
                c.Phone,
                c.Gender
            });
            return Ok(result);
        }

        // GET api/sync/status — retourne les statistiques de la base locale
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var stats = _localDb.GetStats();
            return Ok(new
            {
                localOrders    = stats.Orders,
                localBicycles  = stats.Bicycles,
                localSellers   = stats.Sellers,
                localCustomers = stats.Customers,
                lastSyncedAt   = stats.LastSyncedAt?.ToString("o")
            });
        }

        // POST api/sync/full — synchronise toute la base locale à partir de la base centrale
        [HttpPost("full")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FullSync()
        {
            var orders = await _db.Orders
                .Include(o => o.Seller)
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.Bicycles)
                .ToListAsync();

            foreach (var order in orders)
                _localDb.UpsertOrder(order);

            var bicycles = await _db.Bicycles.Include(b => b.Order).Include(b => b.Shop).ToListAsync();
            foreach (var bicycle in bicycles)
                _localDb.UpsertBicycle(bicycle);

            var sellers = await _db.Sellers.ToListAsync();
            foreach (var seller in sellers)
                _localDb.UpsertSeller(seller);

            var customers = await _db.Customers.ToListAsync();
            foreach (var customer in customers)
                _localDb.UpsertCustomer(customer);

            return Ok(new
            {
                message = "Synchronisation complète réussie.",
                syncedAt = DateTime.UtcNow.ToString("o"),
                counts = new
                {
                    orders = orders.Count,
                    bicycles = bicycles.Count,
                    sellers = sellers.Count,
                    customers = customers.Count
                }
            });
        }
    }
}
