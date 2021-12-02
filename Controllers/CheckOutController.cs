using Art_Gallery.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Art_Gallery.Controllers
{
    public class CheckOutController : Controller
    {


        Art_GalleryEntities Artdb = new Art_GalleryEntities();

        //
        // GET: /Checkout/AddressAndPayment
        public ActionResult ShippingDetails()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult ShippingDetails(FormCollection values)
        {
            Tbl_Order order = new Tbl_Order();
            TryUpdateModel(order);
            var userid = Convert.ToInt32(Session["User_Id"].ToString());
            var uname = Artdb.Tbl_User.Find(userid).UserName;




            try
            {
                order.Order_Username = uname;
                order.Order_Date = DateTime.Now;
              


                //Save Order
                //Artdb.Tbl_Order.Add(order);
                //Artdb.SaveChanges();

                //Process the order
                var cart = ShoppingCart.GetCart(this.HttpContext);
                cart.CreateOrder(order);

                return RedirectToAction("Complete",
                    new { id = order.Order_Id });


            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }

        }


        public ActionResult Complete(int id)
        {

            var userid = Convert.ToInt32(Session["User_Id"].ToString());
            var uname = Artdb.Tbl_User.Find(userid).UserName;
            // Validate customer owns this order
            bool isValid = Artdb.Tbl_Order.Any(
                o => o.Order_Id == id &&
                o.Order_Username == uname);
            ViewBag.Order = id;

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
        // GET: CheckOut
        public ActionResult Index()
        {
            return View();
        }
    }
}