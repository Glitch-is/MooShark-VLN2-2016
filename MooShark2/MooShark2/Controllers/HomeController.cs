using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MooShark2;
using MooShark2.Models;
using MooShark2.Attributes;
using MvcSiteMapProvider.Web.Html.Models;

namespace MooShark2.Controllers
{
    public class HomeController : Controller
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.AssignmentService AssServ = new Services.AssignmentService();
        private Services.CourseService CoursServ = new Services.CourseService();
        private Services.LoginService loginService = new Services.LoginService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();
		/* /Index
		 * This renders the Homepage of MooShark which for example display all courses a user is signed in
		 * and all assigntment that a user has due.
		 */
        [IsStudent]
        public ActionResult Index()
        {
			//Aquiring the current user to display his name and profile picture in the top right corner
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            HomeViewModel startInfo;
			//Render different info depending on if the user is a student or not.
            if(user.roleId == 1)
            { 
                startInfo = new HomeViewModel { assignments = AssServ.GetAssignmentsOfUserHome(userId) , courses = CoursServ.GetCoursesOfUser(userId), gradingService = new Services.GradingService(), account = user };
            }
            else
            {
                startInfo = new HomeViewModel { assignments = AssServ.GetAssignmentsOfTeacherHome(userId), courses = CoursServ.GetCoursesOfTeacher(userId) , gradingService = new Services.GradingService(), account = user};
            }
			//render the homepage
            return View(startInfo);
        }
		/* /About
		 * This renders the About page which holds general info about the site
		 */
        [IsStudent]
        public ActionResult About()
        {

			//Aquiring the current user to display his name and profile picture in the top right corner
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            ViewBag.Message = "Your application description page.";
			//rendering the aboutpage
            return View();
        }
		/* /Contact
		 * This page was not implemented fully
		 */
        [IsStudent]
        public ActionResult Contact()
        {
			//Aquiring the current user to display his name and profile picture in the top right corner
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            ViewBag.Message = "Your contact page.";
			//renderring the contact page
            return View();
        }

    }
}
