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
    public class OrdersController : Controller
    {
        private EntitiesContext db = new EntitiesContext();

        public List<long> ListBicycleId()
        {
            List<long> items = new List<long>();

            foreach (var temps in db.Bicycles.Select(b => b.Id))
            {

                items.Add(temps);
            }

            //List<SelectListItem> items = new List<SelectListItem>();

            

            //ViewBag.BicycleId = items;

            return items;

        }

        //GET: Orders menu
        [Route("Orders menu")]
        public ActionResult Crud()
        {
            OrdersDbInitialize();
            return View();
        }

        private async Task<ActionResult> OrdersDbInitialize()
        {
            Shop shop1 = new Shop();
            for (int i = 1; i < 5; i++)
            {
                Seller seller1 = new Seller();
                Customer customer1 = new Customer();

                Order order = new Order();
                order.Date = DateTime.Now;
                order.Discount = 0.1F;
                order.Seller = seller1;
                order.Customer = customer1;
                order.Shop = shop1;
                order.UseLoyaltyPoint = null;
                //order.LoyaltyPoint = 5 + i;
                //order.LoyaltyPointUsed = 0;
                //order.LoyaltyPointEarned = 0;
                //order.TotalLoyaltyPoint = order.LoyaltyPointCalculated();
                //order.SumFreeTax = order.SumFreeTaxCalculated();
                order.Tax = 0.2F;
                order.ShippingCost = 0;
                order.PayMode = "CB";
                //order.TotalAmount = order.TotalAmountCalculated();
                db.Orders.Add(order);
                //this.SaveChanges();
            }
            await db.SaveChangesAsync();
            return View();
        }



        // GET: Orders
        [Route("Orders index")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdOrder,Quantity,SumFreeTax,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdOrder,Quantity,SumFreeTax,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
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
