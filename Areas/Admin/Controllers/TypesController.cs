using NUTRI_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Type = NUTRI_Project.Models.Type;
using System.Data.Entity.Validation;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class TypesController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/Types
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Types.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Types/Create  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Type type)
        {
             if (ModelState.IsValid)
                {                  
                    db.Types.Add(type);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }  
            return View(type);
        }

        // POST: Admin/Types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Type type)
        {
            if (ModelState.IsValid)
            { 
                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(type);
        }

        // GET: Admin/Types/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Type type = db.Types.Find(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: Admin/Types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Type type = db.Types.Find(id);
            db.Types.Remove(type);           
            db.SaveChanges();           
            return RedirectToAction("Index");
        }
    }
}