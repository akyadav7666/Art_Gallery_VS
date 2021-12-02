using Art_Gallery.Models;
using Art_Gallery.Repository;
using PagedList;
using PagedList.Mvc;
using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Art_Gallery.Models.Home;
using System.Web.Security;

namespace Art_Gallery.Controllers
{
    
    public class AdminController : Controller
    {

        Art_GalleryEntities db = new Art_GalleryEntities();
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();


        public ActionResult Logout()
        {
            Session["Admin_Id"] = null;
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Admin_Login", "Admin");
        }
        public ActionResult Orders()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Order>().GetProduct());
        }

        public ActionResult Dashboard()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            var tables = new HomeIndexViewModel
            {

                Tbl_Category = db.Tbl_Category.ToList(),
                Tbl_Art = db.Tbl_Art.ToList(),

            };
            return View(tables);
        }
        public ActionResult OrderDetails()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View(_unitOfWork.GetRepositoryInstance<Tbl_OrderDetail>().GetProduct());
        }
        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.Category_Id.ToString(), Text = item.Category_Name });
            }
            return list;
        }


        //<-------------------------------------------------{  Login Admin  }----------------------------------------------------------------------->
        [HttpGet]
        public ActionResult Admin_Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Admin_Login(Tbl_Admin avm)
        {
            Tbl_Admin ad = db.Tbl_Admin.Where(x => x.Username == avm.Username && x.Admin_Password == avm.Admin_Password).SingleOrDefault();
            if (ad != null)
            {
                Session["Admin_Id"] = ad.Admin_Id.ToString();
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
        }


        //<-------------------------------------------------{  View Category  }----------------------------------------------------------------------->
        // GET: Admin
        public ActionResult ViewCategory(int? page)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            int pagesize = 7, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.Tbl_Category.Where(x => x.Category_Status == 1).OrderByDescending(x => x.Category_Id).ToList();
            IPagedList<Tbl_Category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            // ReSharper disable once Mvc.ViewNotResolved
            return View();
        }

        //<-------------------------------------------------{  Create category  }----------------------------------------------------------------------->
        // GET: Admin/Create
        
        [HttpGet]
        public ActionResult CreateCategory()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            return View();
        }




        // POST: Admin/Create
        [HttpPost]
        public ActionResult CreateCategory(Tbl_Category cvm, HttpPostedFileBase imgfile)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }

            string path = Uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could be not uploaded....";
            }
            else
            {

                Tbl_Category cat = new Tbl_Category();

                cat.Category_Name = cvm.Category_Name;
                cat.Category_Image = path;
                cat.Category_Status = 1;
                //cat.Admin_Id = Convert.ToInt32(Session["AD_Id"].ToString());
                db.Tbl_Category.Add(cat);
                db.SaveChanges();
                ViewBag.error = "Image upload succsess";
                return RedirectToAction("ViewCategory");
            }






            return View();
        }


        //<-------------------------------------------------{ Edit Category  }----------------------------------------------------------------------->

        // GET: Admin/Edit/5
        [HttpGet]
        public ActionResult EditCategory(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Category cat = db.Tbl_Category.Find(id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            return View(cat);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Tbl_Category tbl, HttpPostedFileBase imgfile, int id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            if (ModelState.IsValid)
            {

                string path = Uploadimgfile(imgfile);
                if (path.Equals("-1"))
                {
                    ViewBag.error = "Image could be not uploaded....";
                }
                else
                {

                    var cat = db.Tbl_Category.Single(model => model.Category_Id == tbl.Category_Id);

                    cat.Category_Name = tbl.Category_Name;
                    cat.Category_Image = path;
                    cat.Category_Status = 1;
                    cat.Admin_Id = Convert.ToInt32(Session["Admin_Id"].ToString());

                    db.SaveChanges();
                    ViewBag.error = "Image upload succsess";
                    return RedirectToAction("ViewCategory");
                }
            }

            return View(tbl);
        }
        //<-------------------------------------------------{  Delete Category  }----------------------------------------------------------------------->
        // GET: Admin/Delete/5
        [HttpGet]
        public ActionResult DeleteCategory(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Category cat = db.Tbl_Category.Find(id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            return View(cat);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            try
            {
                Tbl_Category cat = db.Tbl_Category.Find(id);
                db.Tbl_Category.Remove(cat);
                db.SaveChanges();

                return RedirectToAction("ViewCategory");
            }
            catch
            {
                return View("DeleteCategory");
            }
        }
        //<-------------------------------------------------{  Art  }------------------------------------------------------------------------------>
        public ActionResult Art()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Art>().GetProduct());
        }
        public ActionResult EditArt(int id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            ViewBag.CategoryList = GetCategory();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Art>().GetFirstorDefault(id));
        }
        [HttpPost]
        public ActionResult EditArt(Tbl_Art tbl, HttpPostedFileBase file)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Image/Art/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            tbl.Art_Image = pic;//file != null ? pic : tbl.Art_Image;
            tbl.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Art>().Update(tbl);
            return RedirectToAction("Art");
        }
        public ActionResult AddArt()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            ViewBag.CategoryList = GetCategory();
            return View();
        }
        [HttpPost]
        public ActionResult AddArt(Tbl_Art tbl, HttpPostedFileBase file)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Image/Art/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            tbl.User_Id = 1;
            tbl.IsSold = false;
            
            tbl.Art_Image = pic;
            tbl.CreatedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Art>().Add(tbl);
            return RedirectToAction("Art");
        }



        [HttpGet]
        public ActionResult DeleteArt(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Art art = db.Tbl_Art.Find(id);
            if (art == null)
            {
                return HttpNotFound();
            }
            return View(art);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult DeleteArt(int id)
        {
            
                Tbl_Art art = db.Tbl_Art.Find(id);
            Tbl_Wishlist wish = db.Tbl_Wishlist.Where(model => model.Art_Id == id).SingleOrDefault();
            if (wish != null)
            {
                db.Tbl_Wishlist.Remove(wish);
                db.SaveChanges();
            }
            Tbl_Cart cart = db.Tbl_Cart.Where(model => model.Art_Id == id).SingleOrDefault();
            if (cart != null)
            {
                db.Tbl_Cart.Remove(cart);
                db.SaveChanges();
            }
            db.Tbl_Art.Remove(art);
                db.SaveChanges();

                return RedirectToAction("Art");
          
        }







        //<-------------------------------------------------{  Upload File  }----------------------------------------------------------------------->
        public string Uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extention = Path.GetExtension(file.FileName);
                if (extention.ToLower().Equals(".jpg") || extention.ToLower().Equals(".jpeg") || extention.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Image/Category/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Image/Category/" + random + Path.GetFileName(file.FileName);
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg and png format are acceptable');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file');</script>");
                path = "-1";
            }

            return path;
        }
    }
}
