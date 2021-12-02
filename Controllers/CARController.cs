using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Art_Gallery.Models;

namespace Art_Gallery.Controllers
{
    public class CARController : Controller
    {
        private Art_GalleryEntities db = new Art_GalleryEntities();


        public ActionResult UserRoles()
        {
            ViewBag.User_Id = new SelectList(db.Tbl_User, "User_Id", "UserName");
            ViewBag.RoleId = new SelectList(db.Tbl_Roles, "Id", "RoleName");
            return View();
        }

        // GET: CoustomArtRequest
        public ActionResult Index()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View(db.Tbl_CAR.ToList());
        }

        // GET: CoustomArtRequest/Details/5
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
            Tbl_CAR tbl_CAR = db.Tbl_CAR.Find(id);
            if (tbl_CAR == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CAR);
        }

        // GET: CoustomArtRequest/Create
        public ActionResult ArtRequest()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            var my= db.Tbl_UserRole.Where(ac => ac.RoleId == 101).ToList();
            Tbl_User artist = new Tbl_User();
            List<SelectListItem> artists = new List<SelectListItem>();
            foreach(var item in my)
            {
                artists.Add(new SelectListItem { Value = item.User_Id.ToString(), Text = item.Tbl_User.UserName });
            }

            ViewBag.Id = artists;
            //ViewBag.Id = new SelectList(artists, "User_Id", "UserName");
            return View();
        }

        // POST: CoustomArtRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArtRequest( Tbl_CAR tbl_CAR, HttpPostedFileBase file)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            string pic = null;
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/Art_Request/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }
                var artistname = db.Tbl_User.Where(a => a.User_Id == tbl_CAR.Artist_Id).SingleOrDefault();
                Tbl_CAR car = new Tbl_CAR();
                car.CAR_UserName = Session["UserName"].ToString();
                car.CAR_FullName = tbl_CAR.CAR_FullName;
                car.CAR_Phone = tbl_CAR.CAR_Phone;
                car.CAR_Email = tbl_CAR.CAR_Email;
                car.CAR_Image = pic;
                car.Artist_Name = artistname.User_FirstName + "  " + artistname.User_LastName;
                car.CAR_Desc = tbl_CAR.CAR_Desc;
                car.Artist_Id = tbl_CAR.Artist_Id;
                db.Tbl_CAR.Add(car);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tbl_CAR);
        }

        // GET: CoustomArtRequest/Edit/5
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
            Tbl_CAR tbl_CAR = db.Tbl_CAR.Find(id);
            if (tbl_CAR == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CAR);
        }

        // POST: CoustomArtRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CAR_Id,CAR_UserName,CAR_FullName,IsAccepted,IsRejected,Artist_Name,CAR_Image,CAR_Desc,CAR_Email,CAR_Phone")] Tbl_CAR tbl_CAR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_CAR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_CAR);
        }

        // GET: CoustomArtRequest/Delete/5
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
            Tbl_CAR tbl_CAR = db.Tbl_CAR.Find(id);
            if (tbl_CAR == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CAR);
        }

        // POST: CoustomArtRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tbl_CAR tbl_CAR = db.Tbl_CAR.Find(id);
            db.Tbl_CAR.Remove(tbl_CAR);
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
