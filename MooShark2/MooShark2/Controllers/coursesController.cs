using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using Microsoft.AspNet.Session;
using MooShark2.Attributes;

namespace MooShark2.Controllers
{
    
    public class CoursesController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.CourseService CourseService = new Services.CourseService();
        private Services.LoginService loginServ = new Services.LoginService();
        private Utilites.AccountHelper accountHelp = new Utilites.AccountHelper();

        // GET: courses
        [IsStudent]
        public ActionResult Index()
        {
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            List<courses> info;
            if(user.roleId > 1)
            {
                info = CourseService.GetCoursesOfTeacher(userId);
            }
            else
            {
                info = CourseService.GetCoursesOfUser(userId);
            }

            return View(info);
        }

        // GET: courses/GetUsers/5
        [IsStudent]
        public ActionResult GetUsers(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }

            int courseId = Convert.ToInt32(id);
            var users = CourseService.GetCoursesOfUser(courseId);
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        // POST: courses/SearchUsers/5
        [HttpPost]
        public JsonResult SearchUsers(int? id, FormCollection collection)
        {
            if (id == null|| collection["filter"] == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int courseId = Convert.ToInt32(id);
            var users = CourseService.SearchCourseUsers(courseId, collection["filter"]);
            return Json(users);
        }

        // GET: courses/Details/5
        [IsStudent]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }
            courses courses = db.courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(courses);
        }

        // GET: courses/Create
        [IsAdmin]
        public ActionResult Create()
        {
            return View();
        }

        // POST: courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsAdmin]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description")] courses courses)
        {
            if(courses == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.courses.Add(courses);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            account user = accountHelp.getCurrentUser(Session["User"].ToString());

            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(courses);
        }

        // GET: courses/Edit/5
        [IsTeacher]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            courses courses = db.courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(courses);
        }

        // POST: courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsTeacher]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description")] courses courses)
        {
            if(courses == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.Entry(courses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(courses);
        }

        // GET: courses/Delete/5
        [IsAdmin]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            courses courses = db.courses.Find(id);
            if (courses == null)
            {
                return HttpNotFound();
            }
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(courses);
        }

        // POST: courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [IsAdmin]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courses courses = db.courses.Find(id);
            db.courses.Remove(courses);
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
