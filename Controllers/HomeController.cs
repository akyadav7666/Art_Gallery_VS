using System;
using Art_Gallery.Repository;
using Art_Gallery.Models.Home;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Art_Gallery.Models;


namespace Art_Gallery.Controllers
{
    public class HomeController : Controller
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        Art_GalleryEntities ctx = new Art_GalleryEntities();



  


        public ActionResult Art(string search)
        {
            if (search != null)
            {

                return View(ctx.Tbl_Art.Where(x => x.Art_Name.Contains(search)).ToList());
            }

            return View(ctx.Tbl_Art.ToList());
        }

        public ActionResult Category(string search)
        {
            if (search != null)
            {

                return View(ctx.Tbl_Category.Where(x => x.Category_Name.Contains(search) || search == null).ToList());
            }

            return View(ctx.Tbl_Category.ToList());
        }



        public ActionResult Nearby1()
        {
            return View();
        }
       
        public ActionResult Nearby()
        {
            return View();
        }
        

       
        public ActionResult Index(string search)
        {
            
                var tables = new HomeIndexViewModel
                {

                    Tbl_Category = ctx.Tbl_Category.ToList(),
                    Tbl_Art = ctx.Tbl_Art.ToList(),

                };
                return View(tables);
            

        }

        public ActionResult About()
        {
           
                    ViewBag.Message = "Your application description page.";
                   

                    return View();
            

            
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Cart()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("Login");
            }

            return View(ctx.Tbl_Art.ToList());
        }
        public ActionResult getUserList()
        {
            var user = ctx.Tbl_User.ToList();
            return View(user);
        }
  


        //<-----------------------------------------------{   Art Request    }----------------------------------------------------->

        public ActionResult ArtRequest()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            var my = ctx.Tbl_UserRole.Where(ac => ac.RoleId == 101).ToList();
            Tbl_User artist = new Tbl_User();
            List<SelectListItem> artists = new List<SelectListItem>();
            foreach (var item in my)
            {
                artists.Add(new SelectListItem { Value = item.User_Id.ToString(), Text = item.Tbl_User.UserName });
            }

            ViewBag.Id = artists;
            
            return View();
        }

        // POST: CoustomArtRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArtRequest( Tbl_CAR tbl_CAR, HttpPostedFileBase file)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            string pic = null;
            if (ModelState.IsValid)
            {
                Tbl_CAR car = new Tbl_CAR();
                if (file != null)
                {
                    pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/Art_Request/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }

                car.CAR_Image = pic;

                car.CAR_UserName = Session["UserName"].ToString();
                var artistname = ctx.Tbl_User.Where(a => a.User_Id == tbl_CAR.Artist_Id).SingleOrDefault();
                car.CAR_FullName = tbl_CAR.CAR_FullName;
                car.CAR_Phone = tbl_CAR.CAR_Phone;
                car.CAR_Email = tbl_CAR.CAR_Email;
                
                car.CAR_Desc = tbl_CAR.CAR_Desc;
                car.Artist_Id = tbl_CAR.Artist_Id;
                car.Artist_Name = artistname.User_FirstName +"  "+ artistname.User_LastName;
                ctx.Tbl_CAR.Add(car);
                ctx.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tbl_CAR);
        }


        //<-----------------------------------------------{   Whishlist    }----------------------------------------------------->
    

        public ActionResult AddToWishList(int id, string url)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("Login", "Account");
            }



            Tbl_Wishlist wishlist = new Tbl_Wishlist();
            var userid = Convert.ToInt32(Session["User_Id"].ToString());
            var art = ctx.Tbl_Art.Find(id);

            

                if (ctx.Tbl_Wishlist.Any(ac => ac.Art_Id == id && ac.User_Id==userid) )
                {
                    ViewBag.message = "It alredy exist";
                }
                else
                {

                    if (wishlist.Art_Id != art.Art_Id)
                    {
                        wishlist.User_Id = userid;
                        wishlist.Art_Id = id;
                    
                        ctx.Tbl_Wishlist.Add(wishlist);
                        ctx.SaveChanges();
                        return RedirectToAction("Index", "Home");

                    }
                }
            
            return RedirectToAction("About", "Home");
        }

        public ActionResult WishList()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("Login");
            }

            var WishCount = 0;

            Art_GalleryEntities myart = new Art_GalleryEntities();

            var wishlistttt = myart.Tbl_Wishlist.ToList();

            foreach (var item in wishlistttt)
            {
                WishCount = WishCount + 1;
                ViewBag.Wish = WishCount;
            }

            //return View(model);
            return View(wishlistttt);
        }


        public ActionResult RemoveFromWishList(int id)
        {
            Tbl_Wishlist wish = ctx.Tbl_Wishlist.Find(id);
            ctx.Tbl_Wishlist.Remove(wish);
            ctx.SaveChanges();


            return RedirectToAction("WishList");

        }

//<-----------------------------------------------{   Category Art     }----------------------------------------------------->

        
        public ActionResult CategoryArts(int? id)
        {
            Art_GalleryEntities myart = new Art_GalleryEntities();
           


            var pid = id ?? 1;  //if requested id is null, set is as 1

            var category = myart.Tbl_Category.Find(pid);

            if (category == null)  //if category does not exist, return Error
                return View("Error");

            var model = category.Tbl_Art.ToList(); //return list of projects in requested category

            return View(model);
            
        }
    }
}