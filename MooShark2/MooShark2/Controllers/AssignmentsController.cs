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
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace MooShark2.Controllers
{
    public class AssignmentsController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.AssignmentService assignmentService = new Services.AssignmentService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();
        private Services.ProblemService problemsService = new Services.ProblemService();
        
            

        /* GET: Assignments
		 * This function renders the Assignments Index page that
		 * shows a user all assignments he has due in a specific course
	     */	 
        public ActionResult Index(int ? id)
        {
            int courseId = Convert.ToInt32(id);
			//Getting the current user so that we can display his name and profile image in the corer
            account currUser = accountHelper.getCurrentUser(Session.SessionID);
            ViewBag.userName = currUser.name;
            ViewBag.imagePath = currUser.imagePath;
            AssignmentsViewModel assign = new AssignmentsViewModel { assignments = assignmentService.GetAssignmentsOfCourse(id), user = currUser, courseId = courseId ,gradingService = new Services.GradingService()};
            //rendering the Assignment indexpage 
            return View(assign);
            
            
        }

        /* POST: Assignments/AddTeamMember/5
		 * This functions allows you to add a teammember to an assignment
		 * The collection holds the info about which user you want to add
		 * and the id sent is the Id of the assignment you are making the group in.
		 */
        [HttpPost]
        public JsonResult AddTeamMember(int? id, FormCollection collection)
        {
            string status = "";
            int assignmentId = Convert.ToInt32(id);

            if (assignmentService.AddTeamMember(id, accountHelper.getCurrentUser(Session["User"].ToString()).id, Convert.ToInt32(collection["memberId"])))
                status = "Successful";
            else
                status = "Member is already registered to a team";

            return Json(new { status = status });
        }

        /* POST: Assignments/RemoveMember/5
		 * This function removes a user form a team, which user is found
		 * in the FormCollection given.
		 */
        [HttpPost]
        public JsonResult RemoveMember(int? id, FormCollection collection)
        {
            string status = "";
            int assignmentId = Convert.ToInt32(id);

            if (assignmentService.RemoveMember(id, Convert.ToInt32(collection["memberId"])))
                status = "Successful";
            else
                status = "Error";

            return Json(new { status = status });
        }

        // GET: Assignments/Details/5
        public ActionResult Details(int? id)
        {
			//If we get and invalid id we send a Badrequest status code result instead of rendering the view
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            assignment assignment = db.assignment.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // GET: Assignments/Create
        public ActionResult Create()
        {
            return View();
        }
		/*
		 * This function allows you to create an Assignment
		 */
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {
            
                var date = DateTime.Parse(collection["deadline"]);                
                date.ToString("{0:dd-MM-yy HH:mm:ss.fff}", CultureInfo.InvariantCulture);
                assignment newAssignment = new assignment { name = collection["name"], description = collection["compose"], deadline = date, courseId = Convert.ToInt32(collection["courseId"]) };
                db.assignment.Add(newAssignment);
                db.SaveChanges();
                int assignId = db.assignment.Max(x => x.id);

                return Json(new { status = "SuccessAdd", assignmentId = assignId }, JsonRequestBehavior.AllowGet);
                     
        }


        // GET: Assignments/Edit/5
       [HttpGet]
        public ActionResult EditAssignments(int? id)
        {
			//If we get and invalid id we send a Badrequest status code result instead of rendering the view
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            assignment assignment = db.assignment.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }
        //Get assignment details
        [HttpGet]
        public JsonResult GetAssignmentAsJSON(int? id)
        {
			// Here we gather all the info about the assignment with the id we passed into the function
            int assignId = Convert.ToInt32(id);
            assignment assign = assignmentService.GetAssignmentById(assignId);
            return Json(new { name = assign.name, compose = assign.description, deadline = assign.deadline.ToString() }, JsonRequestBehavior.AllowGet);

        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult EditAssignments(FormCollection collection)
        {
			//If we get and invalid id we send a Badrequest status code result instead of rendering the view
            if (collection["id"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int assId = Convert.ToInt32(collection["id"]);

            var edit = db.assignment.Find(assId);
            edit.name = collection["name"];
            edit.description = collection["compose"];
            
            var date = DateTime.Parse(collection["deadline"]);

            date.ToString("{0:dd-MM-yy HH:mm:ss.fff}", CultureInfo.InvariantCulture);

            edit.deadline = date;
            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "SuccessEdit", assignmentId = assId }, JsonRequestBehavior.AllowGet);
        }

        // GET: Assignments/Delete/5
        public ActionResult Delete(int? id)
        {
			//If we get and invalid id we send a Badrequest status code result instead of rendering the view
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            assignment assignment = db.assignment.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }


        /* Post: Json remove/delete assignment.
		 * This function removes the assignment with the id passed form the database 
		 */
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveAssignment(string id)
        {
            int assignID = Convert.ToInt32(id);
			//If we get and invalid id we send a Badrequest status code result instead of rendering the view
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            db.testcases.RemoveRange(db.testcases.Where(a => a.testcaseId == assignID));
            db.clarifications.RemoveRange(db.clarifications.Where(a => a.clarificationId == assignID));
            db.teams.RemoveRange(db.teams.Where(a => a.assignmentId == assignID));
            db.problems.RemoveRange(db.problems.Where(b => b.assignmentId == assignID));
            db.assignment.Remove(db.assignment.Find(assignID));

            db.SaveChanges();

            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            assignment assignment = db.assignment.Find(id);
            db.assignment.Remove(assignment);
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
