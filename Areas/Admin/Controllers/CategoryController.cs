using NUTRI_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/Category
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Categogies.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }

      
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: Admin/Category/Create    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Categogy a)
        {
            if (ModelState.IsValid)
            {
                db.Categogies.Add(a);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(a);
        }

        // POST: Admin/Category/Edit/1
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categogy a = db.Categogies.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Categogy a)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(a).State = EntityState.Modified;
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
            return View(a);
        }

        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categogy a = db.Categogies.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            return View(a);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categogy a = db.Categogies.Find(id);
            db.Categogies.Remove(a);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}