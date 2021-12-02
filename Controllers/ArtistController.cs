using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Art_Gallery.Models;
using Art_Gallery.Repository;

namespace Art_Gallery.Controllers
{
    public class ArtistController : Controller
    {
        private Art_GalleryEntities db = new Art_GalleryEntities();
        GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        // GET: Artist
        public ActionResult Index()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            List<Tbl_User> users = new List<Tbl_User>();
            var user = db.Tbl_UserRole.Where(a => a.RoleId == 101).ToList();
            foreach (var item in user)
            {
                Tbl_User myuser = db.Tbl_User.Where(a => a.User_Id == item.User_Id).SingleOrDefault();

                users.Add(myuser);
            }
            return View(users);
        }

        // GET: Artist/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_User tbl_User = db.Tbl_User.Find(id);
            if (tbl_User == null)
            {
                return HttpNotFound();
            }
            return View(tbl_User);
        }

        // GET: Artist/Create
        public ActionResult Create()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View();
        }

        // POST: Artist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User_Id,User_Email,User_Phone,User_Password,User_Image,IsActive,UserName,User_FirstName,User_LastName")] Tbl_User tbl_User)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                tbl_User.User_Image = "~/Image/User/avatar7.png";
                db.Tbl_User.Add(tbl_User);
                db.SaveChanges();
                Tbl_UserRole role = new Tbl_UserRole();
                //Tbl_User myuser = db.Tbl_User.Where(x => x.UserName == model.UserName);

                Tbl_User myuser = db.Tbl_User.Where(x => x.UserName == tbl_User.UserName).SingleOrDefault();
                role.RoleId = 100;
                role.User_Id = Convert.ToInt32(myuser.User_Id);

                db.Tbl_UserRole.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_User);
        }

        // GET: Artist/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_User tbl_User = db.Tbl_User.Find(id);
            if (tbl_User == null)
            {
                return HttpNotFound();
            }
            return View(tbl_User);
        }

        // POST: Artist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Tbl_User tbl_User, HttpPostedFileBase file)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                string pic = null;
                if (file != null)
                {
                    pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/User/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }
                tbl_User.User_Image = "~/Image/User/" + pic;

                _unitOfWork.GetRepositoryInstance<Tbl_User>().Update(tbl_User);
                return RedirectToAction("Index");
            }
            return View(tbl_User);
        }

        // GET: Artist/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_User tbl_User = db.Tbl_User.Find(id);
            if (tbl_User == null)
            {
                return HttpNotFound();
            }
            return View(tbl_User);
        }

        // POST: Artist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tbl_User tbl_User = db.Tbl_User.Find(id);
            Tbl_UserRole trole = db.Tbl_UserRole.Where(model => model.User_Id == id).SingleOrDefault();
            db.Tbl_UserRole.Remove(trole);
            db.SaveChanges();
            db.Tbl_User.Remove(tbl_User);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
