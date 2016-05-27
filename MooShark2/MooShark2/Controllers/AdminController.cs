using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MooShark2;
using MooShark2.Models;
using MooShark2.Attributes;
using System.Text;

namespace MooShark2.Controllers
{
    
    public class AdminController : Controller
    {
		//Making instances of the services we need to reference in this controller
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.UserService usrService = new Services.UserService();
        private Services.CourseService crsService = new Services.CourseService();
        private Services.LoginService loginService = new Services.LoginService();
        private Utilites.AccountHelper accountHelper = new Utilites.AccountHelper();

        /* GET: Admin
		 * This function renders the Admin panel index page
		 */
        [IsAdmin]
        public ActionResult Index()
        {
			//Getting the current user so that we can display his name and profile image in the corer
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
			//rendering panel
            AdminPanelViewModel panel = new AdminPanelViewModel {users =usrService.GetAllUsers(),courses = crsService.GetAllCourses() };
            return View(panel);
        }
		/* /Admin/ManageUsers
		 * This function sends info about all the users to the controller which then renders the view; 
		 */
        [IsAdmin]
        public ActionResult ManageUsers()
        {
            ManageUsersViewModel allUsers = new ManageUsersViewModel{ users = usrService.GetAllUsers(), currentUser = null };
            
			//Getting the current user so that we can display his name and profile image in the corer
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            return View(allUsers);
        }
		/*/Admin/ManageUsers
		 * This function sends info about all the course to the controller which then renders the view; 
		 */
        [IsAdmin]
        public ActionResult ManageCourses()
        {
            ManageCoursesViewModel info = new ManageCoursesViewModel { courses = crsService.GetAllCourses() , currentUser = null };

			//Getting the current user so that we can display his name and profile image in the corer
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;

            return View(info);
        }
		/* /Admin/CouresUsers/1
		 * This function renders the users of a selected course based on the course id provided
		 */
        [IsAdmin]
        public ActionResult CourseUsers(int ? id)
        {
			//If somehow we recieve a id which is null we redirect the user to the Homepage rather than rendering the view
            if (id == null)
            {
                return RedirectToAction("Index","Home");
            }
			//Getting the current user so that we can display his name and profile image in the corer
            account user = accountHelper.getCurrentUser(Session["User"].ToString());
            int userId = user.id;
            ViewBag.userName = user.name;
            ViewBag.imagePath = user.imagePath;
            int courseID = Convert.ToInt32(id);
            CourseUsersViewModel info = new CourseUsersViewModel {courseId = courseID, enrolled = crsService.GetStudentsOfCourse(courseID), notEnrolled = crsService.GetUsersNotInCourse(courseID) };


            return View(info);
        }
		/* 
		 * This function will return the all relevant info about the current user in a Json object
		 */
        [IsAdmin]
        [HttpGet]
        public JsonResult GetUserAsJSON(int ? id)
        {
			//If we get sent a user id that doesn't excist we send a Json response saying that the request failed
            if (id == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
			//If how ever we get a valid id we gather info from the account service passing the userId as a variable and return a Json object with all general info about the user
            int usrId = Convert.ToInt32(id);
            account usr = usrService.getSpecificUser(usrId);
            return Json(new { name = usr.name, email = usr.email, password = usr.password, roleid = usr.roleId, imagePath = usr.imagePath } , JsonRequestBehavior.AllowGet);

        }
        /*
		 * This function will return all teachers as a dictionary in a Json object
		 */
        [IsAdmin]
        [HttpPost]
        public JsonResult GetAllTeachersJSON()
        {
            List<account> teachers = usrService.GetAllTeachers();
            Dictionary<int, string> teachersDict = new Dictionary<int, string>();
            foreach(var teach in teachers)
            {
                teachersDict[teach.id] = teach.email;
            }
            return Json(new { status = "SuccessGet", teachers = JsonConvert.SerializeObject(teachersDict) }, JsonRequestBehavior.AllowGet);

        }
		/*
		 * This function returns all info about the corse which id corresponds with the id passed to the function
		 * such as : course name , list of teachers and courese description 
		 */
        [IsAdmin]
        [HttpGet]
        public JsonResult GetCourseAsJSON(int? id)
        {
			//If we get sent a course id that doesn't excist we send a Json response saying that the request failed
            if (id == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
			//If how ever we get a valid id we gather all  info about the course in question
            int crsId = Convert.ToInt32(id);
            courses crs = crsService.getSpecificCourse(crsId);
            List<account> teachers = crsService.GetTeacherOfCourse(crsId);
            List<account> allTeachers = crsService.GetTeachersNotInCourse(crsId);
            Dictionary<int, string> teachersDict = new Dictionary<int, string>();
            foreach(var teach in teachers)
            {
                teachersDict[teach.id] = teach.email;
            }
            Dictionary<int, string> allTeachersDict = new Dictionary<int, string>();
            foreach(var teach in allTeachers)
            {
                allTeachersDict[teach.id] = teach.email;
            }
            var jsonstring = Json(new { name = crs.name, description = crs.description, teachers = JsonConvert.SerializeObject(teachersDict), allTeachers = JsonConvert.SerializeObject(allTeachersDict) }, JsonRequestBehavior.AllowGet);
            return (jsonstring);

        } 
		/*
		 * This function creates a user with data from a form and then sends a success message
		 */

        [IsAdmin]
        [HttpPost]
        public JsonResult AddUser( FormCollection collection )
        {
            
			//If we get sent a user id that doesn't excist we send a Json response saying that the request failed
            if (collection["name"] == null)
            {
                 return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            var hash = sha256(collection["password"]);
            account newUser = new account { name = collection["name"], email = collection["email"], password = hash, roleId = Convert.ToInt32(collection["role"]), imagePath = collection["imgPath"] };

            db.account.Add(newUser);
            db.SaveChanges();
            int usrId = db.account.Max(x => x.id);  

            return Json(new { status = "SuccessAdd", userId = usrId}, JsonRequestBehavior.AllowGet);
		}

		/*
		 * This function creates a course with data from a form and then sends a success message
		 */
        
        [IsAdmin]
        [HttpPost]
        public JsonResult AddCourse(FormCollection collection)
        {
            var teacherArr = JsonConvert.DeserializeObject<List<int>>(collection["teachers"]);
            if(teacherArr.Count <= 0 || collection["name"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            
            courses newCourse = new courses { name = collection["name"], description = collection["description"] };

            db.courses.Add(newCourse);
            db.SaveChanges();
            int courseId = db.courses.Max(a => a.id);
            
            //here we take all the teachers assigned to this class form a list within the form and add them to the course
            foreach(int i in teacherArr)
            {
                teachers neu = new teachers { courseId = courseId, userId = i };
                db.teachers.Add(neu);
            }
            db.SaveChanges();

            account firstTeach = db.account.Find(teacherArr[0]);

            return Json(new { status = "SuccessAdd", courseid = courseId, teacherImgPath = firstTeach.imagePath, courseName = newCourse.name, courseDesc = newCourse.description, teacherName = firstTeach.name }, JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function replaces a users current info with the info in the given FormCollection
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult EditUser(FormCollection collection)
        {
			//If we get sent a user id that doesn't excist we send a Json response saying that the request failed
            if(collection["id"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrid = Convert.ToInt32(collection["id"]);

            var edit = db.account.Find(usrid);
            edit.name = collection["name"];
            edit.email = collection["email"];
            edit.password = collection["password"];
            edit.roleId = Convert.ToInt32(collection["role"]);
            edit.imagePath = collection["imgPath"];
            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();
            
            return Json(new { status = "SuccessEdit", userId = usrid }, JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function replaces a courses current info with the info in the given FormCollection
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult EditCourse(FormCollection collection)
        {
            var teacherArr = JsonConvert.DeserializeObject<List<int>>(collection["teachers"]);
			//If we get sent a course id that doesn't excist we send a Json response saying that the request failed
            if (collection["id"] == null && teacherArr.Count <= 0)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int courseid = Convert.ToInt32(collection["id"]);

            var edit = db.courses.Find(courseid);
            db.teachers.RemoveRange(db.teachers.Where(a => a.courseId.Equals(courseid)));

            foreach(int i in teacherArr)
            {
                teachers neu = new teachers { courseId = courseid, userId = i };
                db.teachers.Add(neu);
            }
            



            edit.name = collection["name"];
            edit.description = collection["description"];
            db.Entry(edit).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "SuccessEdit", userId = courseid }, JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function replaces a courses current info with the info in the given FormCollection
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult AddTeacherToCourse(int id)
        {
			//If we get sent a teacher id that doesn't excist we send a Json response saying that the request failed
            if(id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

            var teacher = db.account.Find(id);

            return Json(new { status = "SuccessAdd", teacherId = teacher.id });


        }
		/*
		 * Removes user with given id  
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveUser(string id)
        {
			//If we get sent a user id that doesn't excist we send a Json response saying that the request failed
            if(id == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrid = Convert.ToInt32(id);
            
            account account = db.account.Find(usrid);
            db.account.Remove(account);
            db.SaveChanges();

            return Json( new { status = "successRemove" } , JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function removes a user to a course both the user and the course are in the FormCollection that's passed to the function
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveUserFromCourse(FormCollection collection)
        {
			//If we get sent a course id or user id that doesn't excist we send a Json response saying that the request failed
            if(collection["id"] == null || collection["crsId"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrid = Convert.ToInt32(collection["id"]);
            int crsid = Convert.ToInt32(collection["crsId"]);
            account actualusr = db.account.Find(usrid);

            var usr = (db.students.Where(a => a.courseId == crsid && a.accountId == usrid)).FirstOrDefault();
            db.students.Remove(usr);
            db.SaveChanges();

            return Json(new { status = "successRemove", email = actualusr.email, role = actualusr.roles.name }, JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function Adds a user to a course both the user and the course are in the FormCollection that's passed to the function
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult AddUserToCourse(FormCollection collection)
        {
			//If we get sent a course id or user id that doesn't excist we send a Json response saying that the request failed
            if(collection["id"] == null || collection["crsId"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int usrid = Convert.ToInt32(collection["id"]);
            int crsid = Convert.ToInt32(collection["crsId"]);
            account actualusr = db.account.Find(usrid);
            students neu = new students { accountId = usrid, courseId = crsid };
            
            db.students.Add(neu);
            db.SaveChanges();

            return Json(new { status = "successRemove", email = actualusr.email, role = actualusr.roles.name }, JsonRequestBehavior.AllowGet);
        }
		/*
		 * This function Removes the course that's given in the FormCollection passed to the function
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveCourse(FormCollection collection)
        {
			//If we get sent a course id that doesn't excist we send a Json response saying that the request failed
            if (collection["id"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
            int courseid = Convert.ToInt32(collection["id"]);

            db.students.RemoveRange(db.students.Where(a => a.courseId == courseid));
            db.teachers.RemoveRange(db.teachers.Where(b => b.courseId == courseid));
            db.courses.Remove(db.courses.Find(courseid));
            db.SaveChanges();

            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }
		/*
		 *  This function removes a teacher from a course both the teacher and the course are given in the FormCollection thats passed to the fucntion
		 */
        [IsAdmin]
        [HttpPost]
        [System.Web.Services.WebMethod]
        public JsonResult RemoveTeacherFromCourse(FormCollection collection)
        {
			//If we get sent a course id or teacher id that doesn't excist we send a Json response saying that the request failed
            if(collection["crsid"] == "0" || collection["teachid"] == null)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }


            int teacherid = Convert.ToInt32(collection["teachid"]);
            int courseid = Convert.ToInt32(collection["crsid"]);

            var theTeach = (from teach in db.teachers
                           where teach.courseId.Equals(courseid) && teach.userId.Equals(teacherid)
                           select teach).FirstOrDefault();


            db.teachers.Remove(theTeach);
            db.SaveChanges();

            return Json(new { status = "successRemove" }, JsonRequestBehavior.AllowGet);
        }

		/*
		 * This function encrypts the passwords of our users
		 */
        static string sha256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
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
