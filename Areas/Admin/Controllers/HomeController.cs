using NUTRI_Project.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NUTRI_Project.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];


            if (String.IsNullOrEmpty(sTenDN))
            {

                ViewData["loli1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {

                ViewData["loli2"] = "Phải nhập mật khẩu";
            }
            else
            {
                var ad = db.Admins.SingleOrDefault(n => n.Email == sTenDN && n.Password == sMatKhau);

                if (ad != null)
                {
                    Session["TenDNAdmin"] = ad;                
                    Session["ten"] = ad.Name;                  
                    string preURL = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return View("Login");   
                }
            }
            return View();
        }
        public ActionResult DangXuat()
        {

            Session.Abandon();
            ViewBag.Ten = "";
            return Redirect("Login");
        }


    }
}