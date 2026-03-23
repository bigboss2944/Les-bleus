using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    [Authorize]
    public class BicyclesController : Controller
    {
        private readonly ApplicationDbContext db;
        private const int PageSize = 10;

        public BicyclesController(ApplicationDbContext context)
        {
            db = context;
        }

        // GET: Bicycles — paginated catalog view
        public async Task<IActionResult> Index(int page = 1)
        {
            var bicycles = db.Bicycles.AsQueryable();
            var paginatedList = await PaginatedList<Bicycle>.CreateAsync(bicycles, page, PageSize);
            return View(paginatedList);
        }

        // GET: Bicycles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Bicycle? bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return NotFound();
            }
            return View(bicycle);
        }
    }
}
