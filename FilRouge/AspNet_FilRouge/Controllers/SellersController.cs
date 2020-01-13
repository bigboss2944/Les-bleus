using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNet_FilRouge.Models;

namespace AspNet_FilRouge.Controllers
{
    public class SellersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Sellers
        [Authorize]
        //[RoleValidator]
        public async Task<ActionResult> Index()
        {
            return View(await db.Sellers.ToListAsync());
        }

        // GET: Sellers/Details/5
        [Authorize]
        //[RoleValidator]
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = await db.Sellers.FindAsync(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Sellers/Create
        [Authorize]
        //[RoleValidator]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdSeller,Password,LastName,FirstName")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Sellers.Add(seller);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(seller);
        }

        // GET: Sellers/Edit/5
        [Authorize]
        //[RoleValidator]
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = await db.Sellers.FindAsync(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdSeller,Password,LastName,FirstName")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(seller);
        }

        // GET: Sellers/Delete/5
        [Authorize]
        //[RoleValidator]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = await db.Sellers.FindAsync(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Seller seller = await db.Sellers.FindAsync(id);
            db.Sellers.Remove(seller);
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
