using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Art_Gallery.Models
{
    public class ShoppingCart
    {
        Art_GalleryEntities storeDB = new Art_GalleryEntities();
        //private HttpContextBase context;

        string ShoppingCartId { get; set; }


        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        internal static object GetCart(object httpContext)
        {
            throw new NotImplementedException();
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Tbl_Art art, string id)
        {


            // Get the matching cart and album instances
            var cartItem = storeDB.Tbl_Cart.SingleOrDefault(c => c.Cart_Id == ShoppingCartId
                                                                 && c.Art_Id == art.Art_Id);

            var uid = Convert.ToInt32(id);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Tbl_Cart
                {
                    Art_Id = art.Art_Id,
                    Cart_Id = ShoppingCartId,
                    Cart_Count = 1,
                    CreatedDate = DateTime.Now,
                    User_Id = uid
                };

                storeDB.Tbl_Cart.Add(cartItem);
            }
            else
            {

                // cartItem.Cart_Count++;
            }

            // Save changes
            storeDB.SaveChanges();
        }




        

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.Tbl_Cart.Single(cart => cart.Cart_Id == ShoppingCartId
                                                           && cart.Record_Id == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Cart_Count > 1)
                {
                    cartItem.Cart_Count--;
                    itemCount = Convert.ToInt32(cartItem.Cart_Count);
                }
                else
                {
                    storeDB.Tbl_Cart.Remove(cartItem);
                }

                // Save changes
                storeDB.SaveChanges();
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeDB.Tbl_Cart.Where(cart => cart.Cart_Id == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Tbl_Cart.Remove(cartItem);
            }

            // Save changes
            storeDB.SaveChanges();
        }

        public List<Tbl_Cart> GetCartItems()
        {
            return storeDB.Tbl_Cart.Where(cart => cart.Cart_Id == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Tbl_Cart
                          where cartItems.Cart_Id == ShoppingCartId
                          select (int?)cartItems.Cart_Count).Sum();

            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            //decimal? total = (from cartItems in storeDB.Tbl_Cart
            //                  where cartItems.Cart_Id == ShoppingCartId
            //                  select (int?)cartItems.Cart_Count * cartItems.Tbl_Art.Art_Price).Sum();
            //return total ?? decimal.Zero;


            var cartItems = storeDB.Tbl_Cart.Where(cart => cart.Cart_Id == ShoppingCartId);
            decimal total = 0;
            foreach (var cartItem in cartItems)
            {
                total = (decimal)(total + cartItem.Tbl_Art.Art_Price);


            }

            return total;
        }

        public int CreateOrder(Tbl_Order order)
        {
            decimal orderTotal = 0;
            var ItemCount = 0;

            var cartItems = GetCartItems();
            

            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                ItemCount = ItemCount + 1;
                var orderDetail = new Tbl_OrderDetail
                {
                    Art_Id = item.Art_Id,
                    Order_Id = order.Order_Id,
                    UnitPrice = item.Tbl_Art.Art_Price,

                };
                var th = storeDB.Tbl_Art.Where(x => x.Art_Id == item.Art_Id).SingleOrDefault();
                th.IsSold = true;
                

                storeDB.Entry(th).State = EntityState.Modified;


                // Set the order total of the shopping cart
                orderTotal = (decimal)(orderTotal + item.Tbl_Art.Art_Price);

                storeDB.Tbl_OrderDetail.Add(orderDetail);

            }
            
        

            // Set the order's total to the orderTotal count
            order.Order_Total = orderTotal;
            order.ItemCount = ItemCount;

            storeDB.Tbl_Order.Add(order);

            // Save the order
            storeDB.SaveChanges();

            // Empty the shopping cart
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.Order_Id;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {

            // var userid = Convert.ToInt32(["User_Id"].ToString());
            // var uname = storeDB.Tbl_User.Find(userid).UserName;
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.Session["User_Id"].ToString()))
                {
                    context.Session["CartSessionKey"] = context.Session["User_Id"].ToString();
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session["CartSessionKey"] = tempCartId.ToString();
                }
            }

            return context.Session["CartSessionKey"].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Tbl_Cart.Where(c => c.Cart_Id == ShoppingCartId);

            foreach (Tbl_Cart item in shoppingCart)
            {
                item.Cart_Id = userName;
            }
            storeDB.SaveChanges();
        }
    }
}