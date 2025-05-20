using NUTRI_Project.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class DOrderController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/DOrder
        public ActionResult Index(int? page,int ?id)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 10;
            if (id !=null)
            {
              return View(db.OrderDetails.Where(n => n.CustomerOrderID == id).ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
            }            
            return View(db.OrderDetails.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }
       
        // GET: Admin/DOrder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID", orderDetail.CustomerOrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderDetail.ProductID);
            ViewBag.Status = new SelectList(db.StatusOrders, "ID", "Name", orderDetail.Status);
            return View(orderDetail);
        }

        // POST: Admin/DOrders/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustomerOrderID,Price,Quantity,Amount,ProductID,Status,Note")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "ID", orderDetail.CustomerOrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}