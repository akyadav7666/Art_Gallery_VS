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
    public class RFController : Controller
    {
        private Art_GalleryEntities db = new Art_GalleryEntities();

        // GET: RF
        public ActionResult Index()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View(db.Tbl_Feedback.ToList());
        }

        // GET: RF/Details/5
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
            Tbl_Feedback tbl_Feedback = db.Tbl_Feedback.Find(id);
            if (tbl_Feedback == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Feedback);
        }

        // GET: RF/Create
        public ActionResult Create()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View();
        }

        // POST: RF/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "F_Id,F_Type,F_DYF,F_FirstName,F_LastName,F_Phone,F_Email,F_UserName")] Tbl_Feedback tbl_Feedback)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                
                db.Tbl_Feedback.Add(tbl_Feedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_Feedback);
        }

        // GET: RF/Edit/5
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
            Tbl_Feedback tbl_Feedback = db.Tbl_Feedback.Find(id);
            if (tbl_Feedback == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Feedback);
        }

        // POST: RF/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "F_Id,F_Type,F_DYF,F_FirstName,F_LastName,F_Phone,F_Email,F_UserName")] Tbl_Feedback tbl_Feedback)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_Feedback);
        }

        // GET: RF/Delete/5
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
            Tbl_Feedback tbl_Feedback = db.Tbl_Feedback.Find(id);
            if (tbl_Feedback == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Feedback);
        }

        // POST: RF/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tbl_Feedback tbl_Feedback = db.Tbl_Feedback.Find(id);
            db.Tbl_Feedback.Remove(tbl_Feedback);
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
