using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Attributes;

namespace MooShark2.Controllers
{
    public class GradesController : Controller
    {
        private Services.GradingService  gradingService = new Services.GradingService();

        // GET: Grades/Index/1
        [IsTeacher]
        public ActionResult Index(int ? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }

            int assignmentId = Convert.ToInt32(id);
            Models.GradesViewModel viewModel = new Models.GradesViewModel {
                assignmentId = assignmentId,
                gradingService = gradingService,
                assignmentService = new Services.AssignmentService(),
                grades = gradingService.GetGrades(assignmentId)
            };
            return View(viewModel);
        }

        // POST: Grades/Edit/
        [HttpPost]
        [IsTeacher]
        public JsonResult Edit(FormCollection collection)
        {
            string status = "";
            int gradeId = Convert.ToInt32(collection["gradeId"]);
            int newGrade = Convert.ToInt32(collection["grade"]);
            if (gradingService.EditGrade(gradeId, newGrade))
                status = "successful";
            else
                status = "error";
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}