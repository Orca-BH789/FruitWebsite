using NUTRI_Project.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 10;
            return View(db.CustomerOrders.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }
        public ActionResult Details(int ID)
        {                    
               
             return RedirectToAction("Index", "DOrder", new { id = ID });
          
        }


        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "Name");         
            return View();
        }

        // POST: Admin/Orders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustomerID,CreateAtTime,Amount")] CustomerOrder co)
        {
            if (ModelState.IsValid)
            {
                db.CustomerOrders.Add(co);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "Name", co.CustomerID);
            return View(co);
        }

        // GET: Admin/CustomerOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOrder co = db.CustomerOrders.Find(id);
            if (co == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "Name", co.CustomerID);          
            return View(co);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustomerID,CreateAtTime,Amount")] CustomerOrder co)
        {
            if (ModelState.IsValid)
            {
                db.Entry(co).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "Name", co.CustomerID);
            return View(co);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOrder co = db.CustomerOrders.Find(id);
            if (co == null)
            {
                return HttpNotFound();
            }
            return View(co);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CustomerOrder co)
        {
            co = db.CustomerOrders.Find(id);
            db.CustomerOrders.Remove(co);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        }
    }
