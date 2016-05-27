using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Services
{
    public class GradingService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        public int GetGrade(int assignmentId, int userId)
        {
            int? teamId = (from team in db.teams
                          where team.assignmentId == assignmentId && team.userId == userId
                          select team.teamId).FirstOrDefault();

            var grade = db.grades.Where(g => g.assignmentId == assignmentId && g.teamId == teamId).Select(g => g.grade).FirstOrDefault();
            if (grade != null)
                return Convert.ToInt32(grade);
            else
                return 0;
        }

        public bool EditGrade(int gradeId, int newGrade)
        {

            var grade = db.grades.Where(g => g.id == gradeId).FirstOrDefault();
            if (grade != null)
            {
                grade.grade = newGrade;
                db.SaveChanges();
                return true;
            }
            else
                return false;
        }

        public IEnumerable<grades> GetGrades(int assignmentId)
        {

            var grade = db.grades.Where(g => g.assignmentId == assignmentId).ToList();
            return grade;
        }
    }
}