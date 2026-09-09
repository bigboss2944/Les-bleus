using AspNet_FilRouge.Models.Api;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNet_FilRouge.Controllers.Api
{
    /// <summary>
    /// API REST de consultation du catalogue (vélos, vendeurs, clients) pour l'application MAUI.
    /// Alimente à la fois la composition de commande et le cache local de synchronisation.
    /// </summary>
    [ApiController]
    [Route("api/mobile/catalog")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = AppConstants.Roles.Vendeur + "," + AppConstants.Roles.Administrateur)]
    public class MobileCatalogController : ControllerBase
    {
        private readonly IBicycleService _bicycleService;
        private readonly ISellerService _sellerService;
        private readonly ICustomerService _customerService;

        public MobileCatalogController(
            IBicycleService bicycleService,
            ISellerService sellerService,
            ICustomerService customerService)
        {
            _bicycleService = bicycleService;
            _sellerService = sellerService;
            _customerService = customerService;
        }

        // GET api/mobile/catalog/bicycles — vélos disponibles à l'ajout dans une commande
        [HttpGet("bicycles")]
        public async Task<IActionResult> GetAvailableBicycles()
        {
            var bicycles = await _bicycleService.GetAvailableAsync();
            return Ok(bicycles.Select(ToSummary));
        }

        // GET api/mobile/catalog/sellers — tous les vendeurs (affichage des commandes de tous les vendeurs)
        [HttpGet("sellers")]
        public async Task<IActionResult> GetSellers()
        {
            var sellers = await _sellerService.GetAllAsync();
            return Ok(sellers.Select(s => new MobileSellerSummary
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName
            }));
        }

        // GET api/mobile/catalog/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers.Select(c => new MobileCustomerSummary
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email
            }));
        }

        private static MobileBicycleSummary ToSummary(Bicycle b) => new()
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
        };
    }
}
