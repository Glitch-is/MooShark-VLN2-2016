using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Utilites;
using MooShark2.Models;
using MooShark2.Attributes;
using MooShark2.Services;
using MvcSiteMapProvider;
using System.Web.UI;

namespace MooShark2.Controllers
{
    public class ProblemsController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.ProblemService problmService = new Services.ProblemService();
        private Services.AssignmentService assignmentService = new Services.AssignmentService();
        private Services.SubmissionService submissionService = new Services.SubmissionService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();
        private Services.LoginService loginService = new Services.LoginService();
        private Services.ScoreboardService scoreboardService = new Services.ScoreboardService();
        private Utilites.IdeOne ideOne = new Utilites.IdeOne();
        private Services.ClarificationService clarificationService = new Services.ClarificationService();

        // GET: Problem
        [IsStudent]
        public ActionResult Index(int ? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }

            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            int assignmentId = Convert.ToInt32(id);
            int courseId = assignmentService.GetCourseId(id);
            List<account> teamm = assignmentService.GetTeamMembers(id, accountHelper.getCurrentUser(Session["User"].ToString()).id);
            account currUser = accountHelper.getCurrentUser(Session["User"].ToString());
            ProblemsViewModel viewModel = new ProblemsViewModel { probs = problmService.GetProblemsOfAssignment(id), aid = assignmentId, cid = courseId, teamMembers = teamm, currentUser = currUser};
            return View(viewModel);

        }

        [IsStudent]
        public ActionResult Problem(int ? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Models.ProblemViewModel info = new Models.ProblemViewModel { prob = problmService.GetProblemById(id), currentUser = accountHelper.getCurrentUser(Session["User"].ToString()) };
            
            if (info.prob == null)
            {  
                // Temp:: redirect to default to skip errors
                return RedirectToAction("Index", "Home");
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(info);
        }

        [IsStudent]
        public ActionResult Scoreboard(int ? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int assignmentId = Convert.ToInt32(id);
            assignment assign = assignmentService.GetAssignmentById(assignmentId);
            List<team> teams = scoreboardService.GetTeamsOfAssignment(assignmentId);
            List<problem> probs = problmService.GetProblemsOfAssignment(assignmentId);

            List<ProblemSuccessHolder> tableData = new List<ProblemSuccessHolder>();
            
            foreach(var team in teams)
            {
                List<bool> success = new List<bool>();
                foreach(var prob in probs)
                {
                    success.Add(scoreboardService.CheckIfDone(prob.id, team.id));   
                }
                tableData.Add(new ProblemSuccessHolder { name = team.name, success = success});
            }
            ScoreboardViewModel info = new ScoreboardViewModel { assignment = assign, probs = probs.Count , tableData = tableData };

            return View(info);
        }

        // GET: Problem/Details/5
        [IsStudent]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            problem problem = db.problem.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(problem);
        }

        // POST: Problems/SubmitCode/5
        [HttpPost]
        [IsStudent]
        public JsonResult SubmitCode(FormCollection collection)
        {
            if (collection == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            int problemId = Convert.ToInt32(collection["probId"]);
            account currUser = accountHelper.getCurrentUser(Session.SessionID);
            int submissionId = submissionService.SubmitCode(problemId, currUser.id, currUser.name, Server.UrlDecode(collection["code"]), Convert.ToInt32(collection["lang"]));
            // Start evaluating test cases in a new thread
            submissionService.TestCaseInit(problemId, submissionId, currUser.id);

            return Json(new { submissionId = submissionId});
        }

        // GET: Problem/Create
        [IsTeacher]
        public ActionResult Create()
        {
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View();
        }

        // POST: Problem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [IsTeacher]
        public JsonResult Create(int? id, FormCollection collection)
        {
            if (id == null || collection["name"] == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            var a = collection["weight"];
            int i = 0;
            int.TryParse(a, out i);
            Nullable<int> result = new Nullable<int>(i);

            problem newProblem = new problem { name = collection["name"], description = collection["description"], weight = result, inputDescription = collection["inputdescription"], outputDescription = collection["outputdescription"], sampleInput = collection["sampleinput"], sampleOutput = collection["sampleoutput"] };
            db.problem.Add(newProblem);
            db.SaveChanges();
            problems newConnection = new problems { problemId = db.problem.Max(x => x.id), assignmentId = Convert.ToInt32(id) };
            db.problems.Add(newConnection);
            db.SaveChanges();
            int prblmId = db.problem.Max(x => x.id);

            return Json(new { status = "SuccessAdd", problemId = prblmId }, JsonRequestBehavior.AllowGet);
        }

        [IsStudent]
        [HttpGet]
        public JsonResult GetLanguages()
        {
            return Json(ideOne.getAllLanguages(), JsonRequestBehavior.AllowGet);
        }

        [IsTeacher]
        [HttpGet]
        public JsonResult GetProblemAsJSON(int? id)
        {
            int prbId = Convert.ToInt32(id);
            problem prb = problmService.GetProblemById(prbId);
            return Json(new { name = prb.name, description = prb.description, weight = prb.weight, sampleinput = prb.sampleInput, sampleoutput = prb.sampleOutput }, JsonRequestBehavior.AllowGet);

        }

        [IsTeacher]
        [HttpGet]
        public JsonResult GetTestCaseAsJSON(int? id)
        {
            int testId = Convert.ToInt32(id);
            testcase test = problmService.GetTestCaseById(testId);
            return Json(new { input = test.input, output = test.output }, JsonRequestBehavior.AllowGet);

        }

        [IsTeacher]
        public ActionResult EditProblem(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            problem problem = problmService.GetProblemById(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            EditProblemViewModel info = new EditProblemViewModel { prob = problem, cases = problmService.getTestCasesOfProblem(id) };
            return View(info);
        }

        [IsTeacher]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult EditProblem(FormCollection collection)
        {
            if (collection["id"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int prbid = Convert.ToInt32(collection["id"]);

            var edit = db.problem.Find(prbid);
            edit.name = collection["name"];
            edit.description = collection["description"];
            var a = collection["weight"];
            int i = 0;
            int.TryParse(a, out i);
            Nullable<int> result = new Nullable<int>(i);
            edit.weight = result;
            edit.sampleInput = collection["sampleinput"];
            edit.sampleOutput = collection["sampleoutput"];
            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "SuccessEdit", problemId = prbid }, JsonRequestBehavior.AllowGet);
        }

        [IsTeacher]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult DeleteProblem(string id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int prbId = Convert.ToInt32(id);

            problem prob = db.problem.Find(prbId);
            problems prob2 = problmService.GetConnectionById(prbId);
            int aId = prob2.id; //because errors
            problems probs = db.problems.Find(aId);
            List<testcase> cases = problmService.getTestCasesOfProblem(prbId);

            foreach(var item in cases)
            {
                db.testcaseOutput.RemoveRange(db.testcaseOutput.Where(b => b.testcaseId == item.id));
            }
            foreach(var item in cases)
            {
                int bId = item.id;
                testcase tst = db.testcase.Find(bId);
                db.testcase.Remove(tst);
            }
            db.testcases.RemoveRange(db.testcases.Where(b => b.problemId == prbId));

            List<clarification> clars = clarificationService.getClarificationsOfProblem(prbId);

            foreach(var item in clars)
            {
                db.clarificationAnswer.RemoveRange(db.clarificationAnswer.Where(b => b.clarificationId == item.id));
            }
            foreach(var item in clars)
            {
                int cId = item.id;
                clarification clr = db.clarification.Find(cId);
                db.clarification.Remove(clr);
            }
            db.clarifications.RemoveRange(db.clarifications.Where(b => b.problemId == prbId));
      
            db.problem.Remove(prob);
            db.problems.Remove(probs);
            db.SaveChanges();

            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }

        // GET: Problem/Delete/5
        [IsTeacher]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            problem problem = db.problem.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(problem);
        }

        // POST: Problem/Delete/5
        [HttpPost, ActionName("Delete")]
        [IsTeacher]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            problem problem = db.problem.Find(id);
            db.problem.Remove(problem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [IsTeacher]
        public JsonResult AddTestCase(int? id, FormCollection collection)
        {
            if (id == null || collection["input"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            testcase newTest = new testcase { input = collection["input"], output = collection["output"] };
            db.testcase.Add(newTest);
            db.SaveChanges();
            testcases newConnection = new testcases { testcaseId = db.testcase.Max(x => x.id), problemId = Convert.ToInt32(id) };
            db.testcases.Add(newConnection);
            db.SaveChanges();
            int testId = db.problem.Max(x => x.id);

            return Json(new { status = "SuccessAdd", testcaseId = testId }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditTestCase(FormCollection collection)
        {
            if (collection["id"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int tstid = Convert.ToInt32(collection["id"]);

            var edit = db.testcase.Find(tstid);
            edit.input = collection["input2"];
            edit.output = collection["output2"];
            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "SuccessEdit", testcaseId = tstid }, JsonRequestBehavior.AllowGet);
        }

        [IsTeacher]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult DeleteTestCase(string id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int tstId = Convert.ToInt32(id);

            testcase test = db.testcase.Find(tstId);
            testcases tests = problmService.GetTestCaseConnectionById(tstId);
            int aId = tests.id; //because errors
            testcases tests2 = db.testcases.Find(aId);

            db.testcaseOutput.RemoveRange(db.testcaseOutput.Where(b => b.testcaseId == tstId));
            db.testcase.Remove(test);
            db.testcases.Remove(tests2);
            db.SaveChanges();

            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }

        /*  protected override void Dispose(bool disposing)
          {
              if (disposing)
              {
                  db.Dispose();
              }
              base.Dispose(disposing);
          }*/
    }
}
