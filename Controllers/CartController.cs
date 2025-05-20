using NUTRI_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models.Payments;

namespace NUTRI_Project.Controllers
{
    public class CartController : Controller
    {
        QLFruitEntities db = new QLFruitEntities();
        // GET: Cart    
        public ActionResult Index()
        {
            return View();
        }
        public List<Cart> TakeCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddCart(int ms, string url)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.Find(n => n.ProductID == ms);
            if (sp == null)
            {
                var product = db.Products.FirstOrDefault(n => n.ID.Equals(ms));
                sp = new Cart();
                sp.ProductID = ms;
                sp.quantity = 1;
                sp.Name = product.Name;
                sp.Pice = product.Price;
                sp.Amount = (int)sp.Pice * (int)sp.quantity;
                lstCart.Add(sp);
            }
            else
            {
                sp.quantity++;
                sp.Amount = (int)sp.Pice * (int)sp.quantity;
            }
            return Redirect(url);
        }
        private int SumCart()
        {
            int sum = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                sum = (int)lstCart.Sum(n => n.quantity);
            }
            return sum;
        }
        private double Total()
        {
            double sum = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                sum = (int)lstCart.Sum(n => n.Amount);
            }
            return sum;
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = TakeCart();
            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return View(lstCart);
        }
        public ActionResult DeleteCart(int? iProductID)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.SingleOrDefault(n => n.ProductID == iProductID);
            if (sp != null)
            {
                lstCart.RemoveAll(n => n.ProductID == iProductID);
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Cart");
        }
        public ActionResult Cartpartial()
        {
            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return PartialView();
        }
        public ActionResult UpdateCart(int? iProductID, FormCollection f)
        {
            List<Cart> lstCart = TakeCart();
            Cart sp = lstCart.SingleOrDefault(n => n.ProductID == iProductID);
            if (sp != null)
            {
                sp.quantity = int.Parse(f["quantity"].ToString());
                sp.Amount = (int)sp.Pice * (int)sp.quantity;
            }
            return RedirectToAction("Cart");
        }


        public ActionResult DeleteCart1()
        {
            List<Cart> lstCart = TakeCart();
            lstCart.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Order()
        {
            if (Session["TenDN"] == null || Session["TenDN"].ToString() == "")
            {
                return Redirect("~/User/Login");
            }
           
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Cart> lstCart = TakeCart();
            ViewBag.SumCart = SumCart();
            ViewBag.Total = Total();
            return View(lstCart);
        }

        public ActionResult OrderPay()
        {
            int CustomerID = Convert.ToInt32((Session["TenDN"] as Customer).ID.ToString());
            //Create New Order
            CustomerOrder ddh = new CustomerOrder();
            ddh.CustomerID = CustomerID;
            ddh.CreateAtTime = DateTime.Now;
            ddh.Amount = (int)Total();
            db.CustomerOrders.Add(ddh);
            db.SaveChanges();
            Session["DH"] = (int)Session["DH"] + 1;

            string a = db.CustomerOrders.OrderByDescending(n => n.ID).FirstOrDefault().ID.ToString();

            //Create new Order Detail
            List<Cart> lstCart = TakeCart();

            foreach (var item in lstCart)
            {
                OrderDetail order = new OrderDetail();
                order.CustomerOrderID = Convert.ToInt32(a);
                order.ProductID = item.ProductID;
                order.Quantity = item.quantity;
                order.Price = item.Pice;
                order.Amount = (int)item.Amount;
                order.Status = 1;
                order.Note = "Thanh Toán qua COD";          
                db.OrderDetails.Add(order);
            }            
            db.SaveChanges();
            Session["Check"] = 1;
            Session["Cart"] = "";
            return RedirectToAction("OrderPay", "Cart");
        }

        #region Thanh toán vnpay
        public ActionResult Payment()
        {           
            string url = ConfigurationManager.AppSettings["vnp_Url"];
            string returnUrl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            VnPayLibrary pay = new VnPayLibrary();
            List<Cart> temp = (List<Cart>)Session["Cart"];
            var total = temp.Sum(x => x.quantity * x.Pice * 100);
 

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", total.ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", "NCB"); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);

        }
        #endregion
      
            public ActionResult PayConfirm()
            {
                if (Request.QueryString.Count > 0)
                {
                    string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuỗi bí mật
                    var vnpayData = Request.QueryString;
                    VnPayLibrary pay = new VnPayLibrary();

                    //lấy toàn bộ dữ liệu được trả về
                    foreach (string s in vnpayData)
                    {
                        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        {
                            pay.AddResponseData(s, vnpayData[s]);
                        }
                    }

                    long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                    long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                    string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                    string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                    bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                    if (checkSignature)
                    {
                        if (vnp_ResponseCode == "00")
                        {
                            ViewBag.Message = "Successfully " + orderId + " | Code: " + vnpayTranId;
                            Session["Cart"] = null;
                            Session["Check"] = 1;

                        }
                        else
                        {
                            ViewBag.Message = "Error with this order " + orderId + " | Code: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                            Session["Check"] = null;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Proccessing Successfully!!!";
                        Session["Check"] = null;
                    }
                    int CustomerID = Convert.ToInt32((Session["TenDN"] as Customer).ID.ToString());
                    //Create New Order
                    CustomerOrder ddh = new CustomerOrder();
                    ddh.CustomerID = CustomerID;
                    ddh.CreateAtTime = DateTime.Now;
                    ddh.Amount = (int)Total();
                    db.CustomerOrders.Add(ddh);
                    db.SaveChanges();

                    string a = db.CustomerOrders.OrderByDescending(n => n.ID).FirstOrDefault().ID.ToString();

                    //Create new Order Detail
                    List<Cart> lstCart = TakeCart();

                    foreach (var item in lstCart)
                    {
                        OrderDetail order = new OrderDetail();
                        order.CustomerOrderID = Convert.ToInt32(a);
                        order.ProductID = item.ProductID;
                        order.Quantity = item.quantity;
                        order.Price = item.Pice;
                        order.Amount = (int)item.Amount;
                        if (Session["Check"] != null) { order.Status = 5; } else { order.Status = 6; }
                        order.Note = "Đã Thanh toán qua VNPAY";
                        db.OrderDetails.Add(order);
                    }
                    db.SaveChanges();
                }

                return View();
            }

        }
}