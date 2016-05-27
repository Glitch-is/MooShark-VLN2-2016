using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MooShark2.Models;

namespace MooShark2.Services
{
    
    class CourseService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        public List<courses> GetAllCourses()
        {
            var ret = (from course in db.courses
                       select course).ToList();
            return ret;
        }
        public courses getSpecificCourse(int id)
        {
            var ret = (from course in db.courses
                       where course.id.Equals(id)
                       select course).FirstOrDefault();
            return ret;
        }


        public List<account> GetUsersOfCourse(int id)
        {
            
            var ret = (from coursesSel in db.courses
                      join studSel in db.students on coursesSel.id equals studSel.courseId
                      join accountSel in db.account on studSel.accountId equals accountSel.id
                      where coursesSel.id.Equals(id)
                      select accountSel ).ToList();
            return ret;
        }
        public List<account> GetStudentsOfCourse(int id)
        {

            var ret = (from coursesSel in db.courses
                       join studSel in db.students on coursesSel.id equals studSel.courseId
                       join accountSel in db.account on studSel.accountId equals accountSel.id
                       where coursesSel.id.Equals(id) && accountSel.roleId.Equals(1)
                       select accountSel).ToList();
            return ret;
        }

        public List<courses> GetCoursesOfUser(int id)
        {
            var ret = (from accSel in db.account
                       join studSel in db.students on accSel.id equals studSel.accountId
                       join coursSel in db.courses on studSel.courseId equals coursSel.id
                       where accSel.id.Equals(id)
                       select coursSel).ToList();
            return ret;
        }

        public List<courses> GetCoursesOfTeacher(int id)
        {
            var ret = (from accSel in db.account
                      join teachSel in db.teachers on accSel.id equals teachSel.userId
                      join courseSel in db.courses on teachSel.courseId equals courseSel.id
                      where accSel.id.Equals(id)
                      select courseSel).ToList();
            return ret;
        }

        public List<AccountQueryModel> SearchCourseUsers(int id, string filter)
        {
            
            var ret = (from coursesSel in db.courses
                      join studSel in db.students on coursesSel.id equals studSel.courseId
                      join accountSel in db.account on studSel.accountId equals accountSel.id
                      where coursesSel.id.Equals(id) && accountSel.name.Contains(filter)
                      select new AccountQueryModel()
                      {
                          id = accountSel.id,
                          name = accountSel.name,
                          imagePath = accountSel.imagePath
                      }).Take(10).ToList();
            return ret;
        }

        public List<account> GetUsersNotInCourse(int id)
        {
            var listOfUsers = GetUsersOfCourse(id);
            var listOfUsersId = listOfUsers.Select(a => a.id);
            var otherIds = db.account.Where(a => !listOfUsersId.Contains(a.id)).Select(a => a.id);
            var ret = (db.account.Where(a => otherIds.Contains(a.id) && a.roleId == 1)).ToList();
            return ret;
        } 

        public List<account> GetTeacherOfCourse(int id)
        {
            var ret = (from coursesSel in db.courses
                       join teach in db.teachers on coursesSel.id equals teach.courseId
                       join accountSel in db.account on teach.userId equals accountSel.id
                       where coursesSel.id.Equals(id) && accountSel.roleId.Equals(2)
                       select accountSel).ToList();
            return ret;
        }
        public List<account> GetTeachersNotInCourse(int id)
        {
            var listOfTeachers = GetTeacherOfCourse(id);
            var listOfTeachersId = listOfTeachers.Select(a => a.id);
            var otherIds = db.account.Where(a => !listOfTeachersId.Contains(a.id)).Select(a => a.id);
            var ret = (db.account.Where(a => otherIds.Contains(a.id) && a.roleId == 2)).ToList();
            return ret;
        }

    }
}
