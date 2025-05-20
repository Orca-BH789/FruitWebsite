using Newtonsoft.Json;
using NUTRI_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NUTRI_Project.Controllers
{
    public class UserController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(FormCollection f)
        {
            string hCaptchaResponse = Request.Form["h-captcha-response"];
            if (!await VerifyHCaptcha(hCaptchaResponse))
            {
                ViewBag.ThongBao = "hCaptcha verification failed.";

            }

            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];


            if (String.IsNullOrEmpty(sTenDN))
            {

                ViewData["loli1"] = "Phải nhập địa chỉ email";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {

                ViewData["loli2"] = "Phải nhập mật khẩu";
            }
            else
            {
                var ad = db.Customers.SingleOrDefault(n => n.Email == sTenDN && n.Password == sMatKhau);               

                if (ad != null)
                {
                    Session["TenDN"] = ad;
                    Session["ten"] = ad.Name;
                    Session["ID"] = ad.ID;
                    string preURL = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                    if (!string.IsNullOrEmpty(preURL) && Url.IsLocalUrl(preURL))
                    {
                        return Redirect(preURL);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBao = "Email hoặc Mật Khẩu không đúng";
                    return View("Login");
                }
            }
            return View();
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Signin(FormCollection f, Customer c)
        {
            string hCaptchaResponse = Request.Form["h-captcha-response"];
            if (!await VerifyHCaptcha(hCaptchaResponse))
            {
                ViewBag.ThongBao = "hCaptcha verification failed.";

            }
            var sTenDN = f["UserName"];
            var sMatKhau = f["MatKhau"];
            var sMatKhauNhapLai = f["MatKhauNL"];
            var sEmail = f["Email"];
            bool isValidEmail = System.Text.RegularExpressions.Regex.IsMatch(sEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var ad = db.Customers.SingleOrDefault(n => n.Email == sEmail);
            if (string.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được để trống";
            }
            else if (string.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Mật khẩu không được để trống";
            }
            else if (string.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }
            else if (sMatKhau != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không khớp";
            }
            else if (string.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được để trống";
            }
            else if (ad != null || !isValidEmail)
            {
                ViewData["err6"] = "Địa chỉ email không hợp lệ.";

            }
            else
            {
                c.Name = sTenDN;
                c.Password = sMatKhau;
                c.Email = sEmail;
                c.Address = "Trong";
                c.Phone = "012345689";
                db.Customers.Add(c);
                db.SaveChanges();
                ViewBag.ThongBao = "Đăng kí thành công";
                return RedirectToAction("Login", "User");
            }

            return this.Signin();
        }

        public ActionResult Info(int id)
        {
            Customer c = db.Customers.Find(id);
            if (c == null)
            {
                return HttpNotFound();
            }
            return View(c);
        }

        [HttpPost]
        public ActionResult Info([Bind(Include = "ID,Name,Password,Email,Phone,Address")] Customer c, FormCollection f)
        {
            var sMatKhauNhapLai = f["MatKhauNL"];
            bool isValidEmail = System.Text.RegularExpressions.Regex.IsMatch(c.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var ad = db.Customers.SingleOrDefault(n => n.Email == c.Email && n.ID != c.ID);

            if (string.IsNullOrEmpty(c.Name))
            {
                ViewData["err2"] = "Họ Tên không được để trống";
            }
            else if (string.IsNullOrEmpty(c.Password))
            {
                ViewData["err4"] = "Mật khẩu không được để trống";
            }
            else if (string.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }
            else if (c.Password != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu nhập lại không khớp";
            }
            else if (string.IsNullOrEmpty(c.Email))
            {
                ViewData["err5"] = "Email không được để trống";
            }
            else if (ad != null || !isValidEmail)
            {
                ViewData["err5"] = "Địa chỉ email không hợp lệ.";
            }
            else if (string.IsNullOrEmpty(c.Phone))
            {
                ViewData["err6"] = "Số điện thoại không được để trống";
            }
            else if (c.Phone.Length != 10)
            {
                ViewData["err6"] = "Vui lòng nhập số điện thoại chính xác";
            }
            else if (string.IsNullOrEmpty(c.Address))
            {
                ViewData["err8"] = "Địa chỉ không được để trống";
            }
            else
            {
                db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.ThongBao = "Cập nhật thành công";
                return RedirectToAction("Index");
            }

            return View("Info", c);
        }

        public ActionResult DangXuat()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        private async Task<bool> VerifyHCaptcha(string hCaptchaResponse)
        {
            var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "secret", "09f1aecb-fe38-4e6a-a926-f855de70273b" },
                { "response", hCaptchaResponse }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://hcaptcha.com/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
            return jsonResponse.success == "true";
        }
    }
}
