using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Models;
using MooShark2.Attributes;
using System.Data.Entity;
using System.Data;
using System.Net;
using System.IO;

namespace MooShark2.Controllers
{

    public class ProfileController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.UserService usrService = new Services.UserService();
        private Services.LoginService loginService = new Services.LoginService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();

        [IsStudent]
        // GET: Profile
        public ActionResult Index()
        {     
            ManageProfileViewModel currentUser = new ManageProfileViewModel { currentUser = accountHelper.getCurrentUser(Session["User"].ToString())};

            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(currentUser);
        }

        [IsStudent]
        [HttpGet]
        public JsonResult GetUserAsJSON(int? id) 
        {
            if (id == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrId = Convert.ToInt32(id);
            account usr = usrService.getSpecificUser(usrId);
            return Json(new {imagePath = usr.imagePath }, JsonRequestBehavior.AllowGet);

        }

        //Change Profile image
        [IsStudent]
        [HttpPost]
        public ActionResult ProfileImageChange(int? id, HttpPostedFileBase file)
        {

            if (file != null)
            {
                int userId = Convert.ToInt32(id);
                var user = db.account.Find(userId);

                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/img/user/"), pic);
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

                user.imagePath = "/img/user/" + pic.Replace("/", "").Replace("\\", "");
                
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            // after successfully uploading redirect the user
            return RedirectToAction("Index", "Profile");
        }

        //Change Password
        [IsStudent]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult ProfilePasswordChange(FormCollection collection)
        {
            if (collection == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrid = Convert.ToInt32(collection["id"]);
            var edit = db.account.Find(usrid);
            edit.password = collection["password"];

            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { status = "SuccessEdit", userId = usrid }, JsonRequestBehavior.AllowGet);
        }
    }
}