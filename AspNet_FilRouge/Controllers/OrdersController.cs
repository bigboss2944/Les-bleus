﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNet_FilRouge.Models;
using Entities;
namespace AspNet_FilRouge.Controllers


{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
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

        public ActionResult SelectIdCategory()
        {
            List<Bicycle> bicycles = db.Bicycles.ToList();

            BicycleOrdersViewModel viewModel = new BicycleOrdersViewModel();

            viewModel.Bicycles = bicycles;

            //List<Bicycle> listeBicycle = new List<Bicycle>
            //{
            //    new Bicycle {
            //        Id=1,
            //        //Shop = "Les bleus",
            //        //bicycle.Order = 1;
            //        //bicycle.Customer = this.Customers.Find(random.Next(1, this.Customers.Count()));
            //        TypeOfBike = "fitness",
            //        Exchangeable = true,
            //        Insurance = true,
            //        Deliverable = true,
            //        Category = "woman",
            //        Reference = "fitness0001",
            //        Size = 1.70F,
            //        Weight = 8.5F,
            //        Color = "yellow and white",
            //        Confort = "",
            //        FreeTaxPrice = 2000F,
            //        Electric = false,
            //        State = "new",
            //        Brand = "les bleus"
            //        },

            //    new Bicycle {
            //        Id=2,
            //        //Shop = "Les bleus",
            //        //bicycle.Order = 1;
            //        //bicycle.Customer = this.Customers.Find(random.Next(1, this.Customers.Count()));
            //        TypeOfBike = "fitness",
            //        Exchangeable = true,
            //        Insurance = true,
            //        Deliverable = true,
            //        Category = "woman",
            //        Reference = "fitness0001",
            //        Size = 1.70F,
            //        Weight = 8.5F,
            //        Color = "yellow and white",
            //        Confort = "",
            //        FreeTaxPrice = 2000F,
            //        Electric = false,
            //        State = "new",
            //        Brand = "les bleus"
            //        },

            //    new Bicycle {
            //        Id=3,
            //        //Shop = "Les bleus",
            //        //bicycle.Order = 1;
            //        //bicycle.Customer = this.Customers.Find(random.Next(1, this.Customers.Count()));
            //        TypeOfBike = "fitness",
            //        Exchangeable = true,
            //        Insurance = true,
            //        Deliverable = true,
            //        Category = "woman",
            //        Reference = "fitness0001",
            //        Size = 1.70F,
            //        Weight = 8.5F,
            //        Color = "yellow and white",
            //        Confort = "",
            //        FreeTaxPrice = 2000F,
            //        Electric = false,
            //        State = "new",
            //        Brand = "les bleus"
            //        },




            //};



            return PartialView("~/Views/Shared/_listBicycleDropDownList.cshtml", viewModel);

        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdOrder,Bicycles,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order, long? BicycleId)
        {
            
           


            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }



            //ViewBag.ListeBicycle = new SelectList(listeBicycle, "Id");
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdOrder,Bicycles,Date,PayMode,Discount,UseLoyaltyPoint,Tax,ShippingCost")] Order order)
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
