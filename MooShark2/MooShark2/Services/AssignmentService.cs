using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooShark2.Services
{

    public class AssignmentService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        public assignment GetAssignmentById(int? id)
        {
            int assignId = Convert.ToInt32(id);
            var assgn = (from assign in db.assignment
                          where assign.id.Equals(assignId)
                          select assign).FirstOrDefault();
            return assgn;
        }

        public List<assignment> GetAssignmentsOfUser(int id)
        {
            var ret = (from accSel in db.account
                       join studSel in db.students on accSel.id equals studSel.accountId
                       join courSel in db.courses on studSel.courseId equals courSel.id
                       join assSel in db.assignment on courSel.id equals assSel.courseId
                       where accSel.id.Equals(id)
                       select assSel).ToList();
            return ret;
        }
        public List<assignment> GetAssignmentsOfTeacher(int id)
        {
            var ret = (from accSel in db.account
                       join teachSel in db.teachers on accSel.id equals teachSel.userId
                       join courSel in db.courses on teachSel.courseId equals courSel.id
                       join assSel in db.assignment on courSel.id equals assSel.courseId
                       where accSel.id.Equals(id)
                       select assSel).ToList();
            return ret;
        }
        public List<assignment> GetAssignmentsOfUserHome(int id)
        {
            var ret = (from accSel in db.account
                       join studSel in db.students on accSel.id equals studSel.accountId
                       join courSel in db.courses on studSel.courseId equals courSel.id
                       join assSel in db.assignment on courSel.id equals assSel.courseId
                       where accSel.id.Equals(id) && assSel.deadline > DateTime.Now
                       select assSel).ToList();
            return ret;
        }
        public List<assignment> GetAssignmentsOfTeacherHome(int id)
        {
            var ret = (from accSel in db.account
                       join teachSel in db.teachers on accSel.id equals teachSel.userId
                       join courSel in db.courses on teachSel.courseId equals courSel.id
                       join assSel in db.assignment on courSel.id equals assSel.courseId
                       where accSel.id.Equals(id) && assSel.deadline > DateTime.Now
                       select assSel).ToList();
            return ret;
        }

        public List<assignment> GetAssignmentsOfCourse(int ? id)
        {
            int courseid = Convert.ToInt32(id);
            var ret = (from courseSel in db.courses
                       join assSel in db.assignment on courseSel.id equals assSel.courseId
                       where assSel.courseId.Equals(courseid)
                       select assSel).ToList();
            return ret;
        }

        public int GetCourseId(int ? id)
        {
            int assignmentId = Convert.ToInt32(id);
            return db.assignment.Where(a => a.id == assignmentId).Select(a => a.courseId).FirstOrDefault();
        }

        public List<account> GetTeamMembers(int ? id, int userId)
        {
            int assignmentId = Convert.ToInt32(id);
            int? teamId = (from team in db.teams
                          where team.assignmentId == assignmentId && team.userId == userId
                          select team.teamId).FirstOrDefault();

            var ret = (from team in db.teams
                       join usr in db.account on team.userId equals usr.id
                       where team.teamId == teamId
                       select usr).ToList();
            return ret;
        }

        public List<string> GetTeamMemberNames(int id, int teamId)
        {

            var ret = (from team in db.teams
                       join usr in db.account on team.userId equals usr.id
                       where team.teamId == teamId
                       select usr.name).ToList();
            return ret;
        }

        public bool AddTeamMember(int ? id, int userId, int memberId)
        {
            int assignmentId = Convert.ToInt32(id);
            int? teamId = (from team in db.teams
                          where team.assignmentId == assignmentId && team.userId == userId
                          select team.teamId).FirstOrDefault();

            // Check if target is already in a group
            if (db.teams.Where(m => m.userId == memberId && m.assignmentId == assignmentId).Count() == 0)
            {
                // Check if we need to create a new team
                if (teamId == null)
                {
                    // Create new team
                    team newTeam = new team();
                    db.team.Add(newTeam);
                    db.SaveChanges();
                    teamId = db.team.Max(x => x.id);
                    teams teamsEntry = new teams { assignmentId = assignmentId, userId = userId, teamId = Convert.ToInt32(teamId) };
                    db.teams.Add(teamsEntry);
                    db.SaveChanges();
                }
                int tid = Convert.ToInt32(teamId);
                db.teams.Add(new teams { assignmentId = assignmentId, userId = memberId, teamId = tid });
                db.SaveChanges();

                return true;
            }
            else
                return false;
        }

        public bool RemoveMember(int ? id, int memberId)
        {
            int assignmentId = Convert.ToInt32(id);

            // Check if target is already in a group
            if (db.teams.Where(m => m.userId == memberId && m.assignmentId == assignmentId).Count() != 0)
            {
                var member = db.teams.Where(m => m.assignmentId == assignmentId && m.userId == memberId).FirstOrDefault();
                if (member == null)
                    return false;
                db.teams.Remove(member);
                db.SaveChanges();

                return true;
            }
            else
                return false;
        }

    }
}
