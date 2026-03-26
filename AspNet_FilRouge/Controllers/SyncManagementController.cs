using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet_FilRouge.Services;

namespace AspNet_FilRouge.Controllers
{
    /// <summary>
    /// Interface d'administration pour la synchronisation de la base locale SQLite.
    /// Accessible uniquement aux administrateurs.
    /// </summary>
    [Authorize(Roles = AppConstants.Roles.Administrateur)]
    public class SyncManagementController : Controller
    {
        private readonly ILocalDbService _localDb;

        public SyncManagementController(ILocalDbService localDb)
        {
            _localDb = localDb;
        }

        // GET: SyncManagement — tableau de bord de synchronisation
        public IActionResult Index()
        {
            var stats = _localDb.GetStats();
            ViewBag.Orders    = stats.Orders;
            ViewBag.Bicycles  = stats.Bicycles;
            ViewBag.Sellers   = stats.Sellers;
            ViewBag.Customers = stats.Customers;
            ViewBag.LastSyncedAt = stats.LastSyncedAt;
            return View();
        }
    }
}
