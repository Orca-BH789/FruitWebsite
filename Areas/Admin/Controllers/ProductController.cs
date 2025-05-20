using NUTRI_Project.Models;
using PagedList;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        public ActionResult Index(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 5;
            return View(db.Products.ToList().OrderBy(n => n.ID).ToPagedList(iPageNum, iPageSize));
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name");
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name");
            return View();

        }
        // POST: Admin/Product/Create    
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Address,CategoryID,TypeID,Info,Price,Image")] Product p, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                string path = "";
                if (PostedFile[0] != null)
                {
                    // Ensure the directory exists
                    var directoryPath = Server.MapPath("~/Image/UploadedFiles/");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    foreach (HttpPostedFileBase file in PostedFile)
                    {
                        string InputFileName = p.ID + "z" + Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(directoryPath, InputFileName);

                        // Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        path += InputFileName + ",";
                    }
                }
                else
                {
                    path = "Default.jpg,";
                }
                path = path.Remove(path.Length - 1);
                p.Image = path;
                p.Info = p.Info.Replace("<p>", "").Replace("</p>", "\n");
                db.Products.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", p.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", p.TypeID);
            return View(p);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product a = db.Products.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", a.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", a.TypeID);
            return View(a);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Address,CategoryID,TypeID,Info,Price,Image")] Product p, HttpPostedFileBase[] PostedFile)
        {
            if (ModelState.IsValid)
            {
                string paths = p.Image;

                if (paths != null)
                {
                    string[] pathss = paths.Split(',');
                    foreach (var item in pathss)
                    {
                        string fullPath = Request.MapPath("~/Image/UploadedFiles/" + item);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                //Modified
                string path = "";
                if (PostedFile[0] != null)
                {
                    foreach (HttpPostedFileBase file in PostedFile)
                    {
                        string InputFileName = p.ID + "z" + Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/Image/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        path += InputFileName + ",";
                    }
                }
                else
                {
                    path = "Default.jpg,";
                }
                path = path.Remove(path.Length - 1);
                p.Image = path;
                p.Info = p.Info.Replace("<p>", "").Replace("</p>", "\n");
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categogies, "ID", "Name", p.CategoryID);
            ViewBag.TypeID = new SelectList(db.Types, "ID", "Name", p.TypeID);
            return View(p);
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product a = db.Products.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }
            return View(a);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product a = db.Products.Find(id);
            db.Products.Remove(a);
            string path = a.Image;
            db.SaveChanges();
            if (path != "Default.jpg")
            {
                string[] paths = path.Split(',');
                foreach (var item in paths)
                {
                    string fullPath = Request.MapPath("~/Image/UploadedFiles/" + item);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}