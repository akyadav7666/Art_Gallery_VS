using System;
using System.Collections.Generic;
using System.Data.Entity;
using Art_Gallery.Repository;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Art_Gallery.Models;
using System.IO;

namespace Art_Gallery.Controllers
{
    public class ManageController : Controller
    {
        Art_GalleryEntities db = new Art_GalleryEntities();
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        public ActionResult Report()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }
            return View();
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(Tbl_Report tbl_Report, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string pic = null;
                if (file != null)
                {
                    pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/Report/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }

                tbl_Report.R_File = pic;
                db.Tbl_Report.Add(tbl_Report);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(tbl_Report);
        }
        public ActionResult Feedback()
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }
            return View();
        }

        // POST: RF/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback([Bind(Include = "F_Id,F_Type,F_DYF,F_FirstName,F_LastName,F_Phone,F_Email")] Tbl_Feedback tbl_Feedback)
        {
            if (ModelState.IsValid)
            {
                tbl_Feedback.F_UserName = Session["UserName"].ToString();
                db.Tbl_Feedback.Add(tbl_Feedback);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(tbl_Feedback);
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


        public ActionResult IsAccepted(int id)
        {

            if (ModelState.IsValid)
            {
                var car = db.Tbl_CAR.Single(model => model.CAR_Id == id);


                car.IsAccepted = true;
                car.IsRejected = false;


                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(id);
        }


        public ActionResult IsRejected(int id)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn","Home");
            }
            if (ModelState.IsValid)
            {
                var car = db.Tbl_CAR.Single(model => model.CAR_Id == id);


                car.IsRejected = true;
                car.IsAccepted = false;

                
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(id);
        }

        public ActionResult Cancel( int id)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }
            if (ModelState.IsValid)
            {
                var car = db.Tbl_CAR.Single(model => model.CAR_Id == id);


                car.IsRejected = null;
                car.IsAccepted = null;


                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(id);
        }
        // GET: Manage
        [HttpGet]
        public ActionResult MyProfile(int id)
        {
            if (Session["User_Id"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var user = db.Tbl_User.Where(x => x.User_Id == id).FirstOrDefault();
            if (user != null)
            {
                TempData["User_Id"] = id;
                TempData.Keep();
                return View(user);
            }
            return View();
        }



        
        [HttpPost]
        
        public ActionResult MyProfile(Tbl_User tbl, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var uid = Convert.ToInt32(Session["User_Id"]);
                string path = Uploadimgfile(file);
                if (path.Equals("-1"))
                {
                    ViewBag.error = "Image could be not uploaded....";
                }
                else
                {

                    var user = db.Tbl_User.Single(model => model.User_Id == uid);

                   
                    user.User_Image = path;
                   

                    db.SaveChanges();
                    ViewBag.error = "Image upload succsess";
                    return RedirectToAction("MyProfile");
                }
            }

            return View(tbl);
        }


        public ActionResult AddArt()
        {
            
            ViewBag.CategoryList = GetCategory();
            return View();
        }
        [HttpPost]
        public ActionResult AddArt(Tbl_Art tbl, HttpPostedFileBase file)
        {
            var uid = Convert.ToInt32(Session["User_Id"]);
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Image/Art/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            tbl.Art_Image = pic;
            tbl.IsSold = false;
            tbl.User_Id = uid;
            
            tbl.CreatedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Art>().Add(tbl);
            return RedirectToAction("Index","Home");
        }



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
                        path = Path.Combine(Server.MapPath("~/Image/User/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Image/User/" + random + Path.GetFileName(file.FileName);
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