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
using MooShark2.Services;
using MvcSiteMapProvider;

namespace MooShark2.Controllers
{
    public class SubmissionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Services.SubmissionService submissionService = new Services.SubmissionService();

        // GET: Submission
        [IsStudent]
        public ActionResult Index()
        {
            return View(db.submissions1.ToList());
        }

        [IsStudent]
        public ActionResult Submission(int? id)
        {
            var submission = submissionService.GetSubmissionById(id);
           // SiteMaps.Current.CurrentNode.Title = problem.name;
            return View(submission);
        }

        // GET: Submission/Details/5
        [IsStudent]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            submission submission = db.submissions1.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            return View(submission);
        }

        // GET: Submission/Create
        [IsStudent]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Submission/SubmitCode
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCode([Bind(Include = "code")] submission submission)
        {
            if (ModelState.IsValid)
            {
                db.submissions1.Add(submission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(submission);
        }

        // GET: Submission/Edit/5
        [IsStudent]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            submission submission = db.submissions1.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            return View(submission);
        }

        // POST: Submission/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code")] submission submission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(submission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(submission);
        }

        // GET: Submission/Delete/5
        [IsStudent]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            submission submission = db.submissions1.Find(id);
            if (submission == null)
            {
                return HttpNotFound();
            }
            return View(submission);
        }

        // POST: Submission/Delete/5
        [HttpPost, ActionName("Delete")]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            submission submission = db.submissions1.Find(id);
            db.submissions1.Remove(submission);
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
