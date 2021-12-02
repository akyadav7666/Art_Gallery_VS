using Art_Gallery.Models;
using Art_Gallery.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace Art_Gallery.Controllers
{
    public class AccountController : Controller
    {
        public new HttpContextBase HttpContext { get; private set; }

        Art_GalleryEntities db = new Art_GalleryEntities();
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        


        public ActionResult UserRoles()
        {
            ViewBag.User_Id = new SelectList(db.Tbl_User, "User_Id", "UserName");
            ViewBag.RoleId = new SelectList(db.Tbl_Roles, "Id", "RoleName");
            return View();
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogIn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogIn(LogInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {


                Tbl_User user = db.Tbl_User.Where(x => x.UserName == model.UserName && x.User_Password == model.Password).SingleOrDefault();
                
                if (user !=null)
                {
                    var uid = user.User_Id;
                    //MigrateShoppingCart(model.UserName);
                    Session["User_Id"] = user.User_Id.ToString();
                    Session["UserName"] = user.UserName.ToString();

                    Tbl_UserRole urole = db.Tbl_UserRole.Where(m => m.User_Id == uid).SingleOrDefault();
                    var roleid = urole.RoleId;
                    var role = db.Tbl_Roles.Find(roleid).RoleName;


                    Session["Role"] = role;

                    
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            Session["User_Id"] = null;
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            //ViewBag.Id = new SelectList(db.Tbl_Roles, "Id", "RoleName");

            ViewBag.Id = GetAll_Roles();
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
           

                if (ModelState.IsValid)
                {
                    Tbl_User user = new Tbl_User();
                    user.User_FirstName = model.FirstName;
                    user.User_LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.User_Email = model.Email;
                    user.User_Phone = model.Phone;
                    user.User_Password = model.Password;
                    user.User_Image = "~/Image/User/avatar7.png";


                    db.Tbl_User.Add(user);

                db.SaveChanges();
                    Tbl_UserRole role = new Tbl_UserRole();
                    //Tbl_User myuser = db.Tbl_User.Where(x => x.UserName == model.UserName);

                    Tbl_User myuser = db.Tbl_User.Where(x => x.UserName == model.UserName).SingleOrDefault();
                    role.RoleId = Convert.ToInt32(model.Id);
                    role.User_Id = Convert.ToInt32(myuser.User_Id);

                    db.Tbl_UserRole.Add(role);
                    db.SaveChanges();



                    if (user.UserName == model.UserName)
                    {
                        //MigrateShoppingCart(model.UserName);

                        FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("LogIn", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(0));
                    }
                }


            // If we got this far, something failed, redisplay form
            return View(model);
            //return RedirectToAction("Register","Account");
        }



       
        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public List<SelectListItem> GetAll_Roles()

        {
            List<SelectListItem> listrole = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_Roles>().GetAllRecords();

            foreach (var item in cat)

            {


                listrole.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.RoleName });



            }

            return listrole;

        }



        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
