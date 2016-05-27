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
    public class TestCaseController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.LoginService loginServ = new Services.LoginService();
        private Utilites.AccountHelper accHelp = new Utilites.AccountHelper();

        // GET: TestCase/Index/1
        [IsStudent]
        public JsonResult Index(int? id)
        {
            if (id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);

            }
            account user = accHelp.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            int testId = Convert.ToInt32(id);
            testcaseOutput testOut = db.testcaseOutput.Where(t => t.id == testId).FirstOrDefault();
            return Json(new { output = testOut.output, expected = testOut.testcase.output, error = testOut.stderr, result = testOut.resultId }, JsonRequestBehavior.AllowGet);
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
