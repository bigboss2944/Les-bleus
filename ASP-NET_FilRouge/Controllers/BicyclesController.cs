using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASP_NET_FilRouge.Models;
using Entities;


namespace ASP_NET_FilRouge.Controllers
{
    public class BicyclesController : Controller
    {
        private EntitiesContext db = new EntitiesContext();

        //GET: Bicycles menu
        [Route("Bicycles menu")]
        public ActionResult Crud()
        {
            return View();
        }

        // GET: Bicycles
        [Route("Bicycles index")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Bicycles.ToListAsync());
        }

        // GET: Bicycles/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bicycle bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return HttpNotFound();
            }
            return View(bicycle);
        }

        // GET: Bicycles/Create
        [Route("Bicycles create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bicycles/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles ="Admin")]
        public async Task<ActionResult> Create([Bind(Include = "Id,TypeOfBike,Category,Reference,FreeTaxPrice,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                db.Bicycles.Add(bicycle);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(bicycle);
        }

        // GET: Bicycles/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bicycle bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return HttpNotFound();
            }
            return View(bicycle);
        }

        // POST: Bicycles/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TypeOfBike,Category,Reference,FreeTaxPrice,Exchangeable,Insurance,Deliverable,Size,Weight,Color,WheelSize,Electric,State,Brand,Confort")] Bicycle bicycle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bicycle).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bicycle);
        }

        // GET: Bicycles/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bicycle bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return HttpNotFound();
            }
            return View(bicycle);
        }

        // POST: Bicycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Bicycle bicycle = await db.Bicycles.FindAsync(id);
            db.Bicycles.Remove(bicycle);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
