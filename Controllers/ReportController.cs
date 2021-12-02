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
    public class ReportController : Controller
    {
        private Art_GalleryEntities db = new Art_GalleryEntities();

        // GET: Report
        public ActionResult Index()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login","Admin");
            }
            return View(db.Tbl_Report.ToList());
        }

        // GET: Report/Details/5
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
            Tbl_Report tbl_Report = db.Tbl_Report.Find(id);
            if (tbl_Report == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Report);
        }

        // GET: Report/Create
        public ActionResult Create()
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            return View();
        }

        // POST: Report/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Tbl_Report tbl_Report, HttpPostedFileBase file)
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/Report/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }

                tbl_Report.R_File = pic;
                db.Tbl_Report.Add(tbl_Report);
                db.SaveChanges();
                return RedirectToAction("Report","Manage");
            }

            return View(tbl_Report);
        }

        // GET: Report/Edit/5
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
            Tbl_Report tbl_Report = db.Tbl_Report.Find(id);
            if (tbl_Report == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Report);
        }

        // POST: Report/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Tbl_Report tbl_Report, HttpPostedFileBase file)
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Image/Report/"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                }

                tbl_Report.R_File = pic;
                db.Entry(tbl_Report).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_Report);
        }

        // GET: Report/Delete/5
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
            Tbl_Report tbl_Report = db.Tbl_Report.Find(id);
            if (tbl_Report == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Report);
        }

        // POST: Report/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Admin_Id"] == null)
            {
                return RedirectToAction("Admin_Login", "Admin");
            }
            Tbl_Report tbl_Report = db.Tbl_Report.Find(id);
            db.Tbl_Report.Remove(tbl_Report);
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
