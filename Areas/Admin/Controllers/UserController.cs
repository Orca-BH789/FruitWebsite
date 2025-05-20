using NUTRI_Project.Models;
using PagedList;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Customers.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }    

        // GET: Admin/User/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Admin/User/Create    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Password,Email,Phone,Address")] Customer ac)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(ac);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ac);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer ac = db.Customers.Find(id);
            if (ac == null)
            {
                return HttpNotFound();
            }
            return View(ac);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Password,Email,Phone,Address")] Customer ac)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ac).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {                    
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            return View(ac);
        }

        // GET: Admin/User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer ac = db.Customers.Find(id);
            if (ac == null)
            {
                return HttpNotFound();
            }
            return View(ac);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer ac = db.Customers.Find(id);
            db.Customers.Remove(ac);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}