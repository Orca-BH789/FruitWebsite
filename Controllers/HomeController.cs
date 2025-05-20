using NUTRI_Project.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUTRI_Project.Controllers
{
    public class HomeController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/Home    
        private List<Product> laysp(int count)
        {
            return db.Products.OrderByDescending(n => n.ID).Take(count).ToList();
        }
        private IPagedList<Product> LaySPMoi(int page, int pageSize)
        {
            return db.Products.OrderByDescending(n => n.ID).ToPagedList(page, pageSize);
        }

        public ActionResult Index(int? page)
        {
            int iPageNum = page ?? 1;
            int iPageSize = 6;
            var pagedSachMoi = LaySPMoi(iPageNum, iPageSize);
            ViewBag.Categories = db.Categogies.ToList();
            return View(pagedSachMoi);
        }
        public ActionResult Shop(int? page)
        {
            int iPageNum = page ?? 1;
            int iPageSize = 6;
            var pagedSachMoi = LaySPMoi(iPageNum, iPageSize);

            return View(pagedSachMoi);
        }

        public ActionResult CaterotyPartial()
        {
            var listC = from a in db.Categogies select a;
            return PartialView(listC);
        }       
        public ActionResult SPTheoCaterory(int id)
        {
            var lay = from s in db.Products where s.CategoryID == id select s;
            return View(lay);
        }
        public ActionResult Detail(int id)
        {
            var sp = from s in db.Products where s.ID == id select s;
            return View(sp.Single());        }

        public ActionResult Contact()
        {
           
            return View();
        }
    }
}