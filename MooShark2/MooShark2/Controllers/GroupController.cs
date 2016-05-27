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

namespace MooShark2.Controllers
{
    public class GroupController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Group
        [IsStudent]
        public ActionResult Index()
        {
            //var groupMembers = db.groupMembers.Include(g => g.team);
            return View(db.groupMembers.ToList());
        }

        // GET: Group/Details/5
        [IsStudent]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            groupMembers groupMembers = db.groupMembers.Find(id);
            if (groupMembers == null)
            {
                return HttpNotFound();
            }
            return View(groupMembers);
        }

        // GET: Group/Create
        [IsStudent]
        public ActionResult Create()
        {
            ViewBag.teamId = new SelectList(db.teams1, "id", "name");
            return View();
        }

        // POST: Group/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,teamId,userId")] groupMembers groupMembers)
        {
            if (groupMembers == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.groupMembers.Add(groupMembers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.teamId = new SelectList(db.teams1, "id", "name", groupMembers.teamId);
            return View(groupMembers);
        }

        // GET: Group/Edit/5
        [IsStudent]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            groupMembers groupMembers = db.groupMembers.Find(id);
            if (groupMembers == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamId = new SelectList(db.teams1, "id", "name", groupMembers.teamId);
            return View(groupMembers);
        }

        // POST: Group/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,teamId,userId")] groupMembers groupMembers)
        {
            if (groupMembers == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.Entry(groupMembers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.teamId = new SelectList(db.teams1, "id", "name", groupMembers.teamId);
            return View(groupMembers);
        }

        // GET: Group/Delete/5
        [IsStudent]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            groupMembers groupMembers = db.groupMembers.Find(id);
            if (groupMembers == null)
            {
                return HttpNotFound();
            }
            return View(groupMembers);
        }

        // POST: Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [IsStudent]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            groupMembers groupMembers = db.groupMembers.Find(id);
            db.groupMembers.Remove(groupMembers);
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
