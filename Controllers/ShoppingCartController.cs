using Art_Gallery.Models;
using Art_Gallery.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace Art_Gallery.Controllers
{
    public class ShoppingCartController : Controller
    {
        Art_GalleryEntities storeDB = new Art_GalleryEntities();

        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {

            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public ActionResult AddToCart(int id)
        {

            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // Retrieve the album from the database
            var addedArt = storeDB.Tbl_Art.SingleOrDefault(art => art.Art_Id == id);

            var uid = Session["User_Id"].ToString();


            ViewBag.carts = storeDB.Tbl_Cart.Where(ac => ac.Tbl_User.User_Id.Equals(uid));

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedArt, uid);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index", "Home");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the album to display confirmation
            string artname = storeDB.Tbl_Cart
                .Single(item => item.Record_Id == id).Tbl_Art.Art_Name;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(artname) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

      
    }
}