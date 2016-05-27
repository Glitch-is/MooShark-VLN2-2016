using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Models;
using MooShark2.Attributes;
using MooShark2.Utilites;
using System.Web.Routing;
using MooShark2.Controllers;

namespace MooShark2.Controllers
{
    public class ClarificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Services.LoginService loginService = new Services.LoginService();
        private Services.ClarificationService ClarifService = new Services.ClarificationService();
        private Services.ProblemService problmService = new Services.ProblemService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();

        // GET: Clarifications
        [IsStudent]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int pId = Convert.ToInt32(id);
            account currUser = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = currUser.id;
            ViewBag.userName = currUser.name;
            ViewBag.imagePath = currUser.imagePath;
            ClarificationsViewModel clar = new ClarificationsViewModel { pId = Convert.ToInt32(id), clarification = ClarifService.GetClarificationsById(id), currentUser = currUser };
            // return View(db.clarifications1.ToList());

            if(clar.clarification.Count() == 0)
            {
                return RedirectToAction("Create", new { id = id});
                 
            }
            else
            {
                return View(clar);
            }
            
        }

        [HttpPost]
        [IsStudent]
        public JsonResult SubmitQuestion(FormCollection collection)
        {
            if (collection["problemId"] == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            account currUser = accountHelper.getCurrentUser(Session["User"].ToString());
            string status = "";
            
            if (ClarifService.AddQuestion(collection["problemId"].ToString(), currUser.id, collection["newQuestion"]) )
                status = "Successful";
            else
                status = "Failed to submit question";

            return Json(new { status = status }, JsonRequestBehavior.AllowGet) ;
        }

        [HttpGet]
        [IsStudent]
        public JsonResult GetClarificationIdAsJSON(int? id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            int clarId = Convert.ToInt32(id);
            return Json(new { clarificationId= clarId }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [IsStudent]
        public JsonResult SubmitAnswer(FormCollection collection)
        {
            if (collection["clarificationId"] == null)
            {
                Console.WriteLine("Epic fail, you suck");
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            account currUser = accountHelper.getCurrentUser(Session["User"].ToString());
            string status = "";

            if (ClarifService.AddAnswer(collection["clarificationId"].ToString(), currUser.id, collection["newAnswer"]))
                status = "Successful";
            else
                status = "Failed to submit answer";

            return Json(new { status = status });
        }

        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveQuestion(string id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int clarId = Convert.ToInt32(id);
            if (!ClarifService.removeQuestion(clarId))
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            
            
            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveAnswer(string id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int clarAnsId = Convert.ToInt32(id);
            if (!ClarifService.removeAnswer(id))
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }


        // GET: Clarifications/Details/5
        [IsStudent]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            clarification clarification = db.clarifications1.Find(id);
            if (clarification == null)
            {
                return HttpNotFound();
            }
            return View(clarification);
        }

        // GET: Clarifications/Create
        [IsStudent]
        public ActionResult Create(int id)
        {
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            ClarificationsEmptyViewModel clar = new ClarificationsEmptyViewModel { pId = Convert.ToInt32(id), currentUser = user };
            
            
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(clar);
        }

        // POST: Clarifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,authorId,message,posted")] clarification clarification)
        {
            if (clarification == null)
            {
                return RedirectToAction("Index","Home");
            }
            
            if (ModelState.IsValid)
            {
                db.clarifications1.Add(clarification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(clarification);
        }

        // GET: Clarifications/Edit/5
        [IsStudent]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            clarification clarification = db.clarifications1.Find(id);
            if (clarification == null)
            {
                return HttpNotFound();
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(clarification);
        }

        // POST: Clarifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,authorId,message,posted")] clarification clarification)
        {
            if (clarification == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (ModelState.IsValid)
            {
                db.Entry(clarification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(clarification);
        }

        // GET: Clarifications/Delete/5
        [IsStudent]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            clarification clarification = db.clarifications1.Find(id);
            if (clarification == null)
            {
                return HttpNotFound();
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(clarification);
        }

        // POST: Clarifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clarification clarification = db.clarifications1.Find(id);
            db.clarifications1.Remove(clarification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
/*
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
