using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Models;
using MooShark2.Attributes;

namespace MooShark2.Controllers
{
    public class SubmissionsController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.SubmissionService subServ = new Services.SubmissionService();
        private Services.LoginService loginServ = new Services.LoginService();
        private Services.ProblemService problemService = new Services.ProblemService();
        private Utilites.AccountHelper accountHelp = new Utilites.AccountHelper();
        

        // GET: Submissions/1
        [IsStudent]
        public ActionResult Index(int? id)
        {
            //grab all submissions if you're a teacher
            //grab your submissions if your groups submissions if you're a student
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }
            account currentUser = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = currentUser.id;
            ViewBag.userName = currentUser.name;
            ViewBag.imagePath = currentUser.imagePath;
            List<submission> subs;
            if(currentUser.roleId > 1)
            {
                subs = db.submission.Where(s => s.problemId == id).ToList();
            }
            else
            {
                int problId = Convert.ToInt32(id);
                subs = subServ.GetSubmissionsOfUserOfProblem(currentUser.id, problId);
            }
            SubmissionsViewModel viewModel = new SubmissionsViewModel { submissions = subs, ideOne = new Utilites.IdeOne() };
            return View(viewModel);
        }

        // GET: Submissions/Submission/1
        [IsStudent]
        public ActionResult Submission(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            int submissionId = Convert.ToInt32(id);
            SubmissionViewModel viewModel = new SubmissionViewModel
            {
                submission = db.submission.Where(s => s.id == id).FirstOrDefault(),
                testcases = db.testcaseOutput.Where(t => t.submissionId == submissionId),
                ideOne = new Utilites.IdeOne()
            };
            account user = accountHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(viewModel);
        }
        
        [IsTeacher]
        public ActionResult Teacher(int? id)
        {
            account currentUser = accountHelp.getCurrentUser(Session["User"].ToString());
            ViewBag.userName = currentUser.name;
            ViewBag.imagePath = currentUser.imagePath;
            int problemId = Convert.ToInt32(id);
            SubmissionTeacherViewModel info = new SubmissionTeacherViewModel { TableData = new List<SubmissionTeacherExtraClass>() };
            List<team> teams = problemService.GetTeamsThatHaveSubmitted(problemId);
            teams = teams.Distinct().ToList();

            foreach(var tm in teams)
            {
                SubmissionTeacherExtraClass tableEntry = new SubmissionTeacherExtraClass { teamName = tm.name, submissionId = subServ.GetTeamsBestSubId(tm.id, problemId) };
                info.TableData.Add(tableEntry);
            }

            return View(info);

        }
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        // GET: Submissions/Download/1
        [IsStudent]
        public FileResult 
            Download(int? id)
        {
            int submissionId = Convert.ToInt32(id);
            var submission = db.submission.Where(s => s.id == submissionId).FirstOrDefault();
            return File(GetBytes(submission.code), System.Net.Mime.MediaTypeNames.Application.Octet, submission.problem.name + "-" + submission.id +GetExt(Convert.ToInt32(submission.languageId)));
        }

        public string GetExt(int lang)
        {
            switch (lang)
            {
                case 7:
                    return ".ada";
                    break;
                case 4:
                case 99:
                case 116:
                    return ".py";
                    break;
                case 17:
                    return ".rb";
                    break;
                case 40:
                    return ".sql";
                    break;
                case 29:
                    return ".php";
                    break;
                case 54:
                    return ".pl";
                    break;
                case 112:
                case 35:
                    return ".js";
                    break;
                case 10:
                    return ".java";
                    break;
                case 11:
                case 81:
                    return ".c";
                    break;
                case 1:
                case 44:
                    return ".cpp";
                    break;
                case 27:
                    return ".cs";
                    break;
                default:
                    return ".cpp";
                    break;

            }
            
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
