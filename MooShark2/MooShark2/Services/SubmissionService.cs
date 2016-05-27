using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MooShark2.Utilites;
using System.Threading.Tasks;
using System.Threading;

namespace MooShark2.Services
{
    public class SubmissionService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Utilites.IdeOne ideone = new Utilites.IdeOne();

        public int SubmitCode(int problemId, int usrId, string userName, string code, int languageId)
        {
            int? assignmentId = (from probs in db.problems
                          where probs.problemId == problemId
                          select probs.assignmentId).FirstOrDefault();

            int? teamId = (from team in db.teams
                          where team.assignmentId == assignmentId && team.userId == usrId
                          select team.teamId).FirstOrDefault();

            // Check if user is in team
            if (teamId == 0)
            {
                // Create a team
                team newTeam = new team { name = userName };
                db.team.Add(newTeam);
                db.SaveChanges();
                teamId = db.team.Max(x => x.id);
                teams teamsEntry = new teams { assignmentId = Convert.ToInt32(assignmentId), userId = usrId, teamId = Convert.ToInt32(teamId) };
                db.teams.Add(teamsEntry);
                db.SaveChanges();
            }

            submission newSubmission = new submission { code = code, problemId = problemId, languageId = languageId };
            db.submission.Add(newSubmission);
            db.SaveChanges();
            int submissionId = db.submission.Max(x => x.id);

            submissions submissionEntry = new submissions { submissionId = submissionId, teamId = Convert.ToInt32(teamId)};
            db.submissions.Add(submissionEntry);
            db.SaveChanges();



            return submissionId;
        }

        public delegate void TestCaseInvoker(int problemId, int submissionId, int userId);

        public void TestCaseInit(int problemId, int submissionId, int userId)
        {
            TestCaseInvoker simpleDelegate = new TestCaseInvoker(RunTestCases);
            simpleDelegate.BeginInvoke(problemId, submissionId, userId, null, null);
        }

        public void RunTestCases(int problemId, int submissionId, int userId)
        {
            var tests = (from testcases in db.testcases
                             join testcase in db.testcase on testcases.testcaseId equals testcase.id
                             where testcases.problemId == problemId
                             select testcase).ToList();

            var subm = db.submission.Where(s => s.id == submissionId).FirstOrDefault();
            subm.correctTestcases = 0;

            bool error = false, wrong = false;

            foreach (var test in tests)
            {
                int ideoneId = ideone.runCode(subm.code, Convert.ToInt32(subm.languageId), test.input);

                testcaseOutput testOut = new testcaseOutput { submissionId = submissionId, testcaseId = test.id, ideoneId = ideoneId, statusId = -1, status = "Pending"};
                db.testcaseOutput.Add(testOut);
                db.SaveChanges();

                Dictionary<string, string> response;
                do
                {
                    Thread.Sleep(5000);
                    response = ideone.getCodeStatus(ideoneId);

                    // Update status
                    testOut.statusId = Convert.ToInt32(response["status"]);
                    db.SaveChanges();

                } while (response["status"] != "0"); // Program has finished

                // Fetch the output and put it in the db
                testOut.statusId = Convert.ToInt32(response["status"]); // Should be 0 now
                testOut.resultId = Convert.ToInt32(response["result"]); // What error occurred
                testOut.output = response["output"]; // STDOUT of the program
                testOut.stderr = response["cmpinfo"]; // STDERR of the program
                // Add time of execution

                // Check if output was correct and update db
                if (testOut.output.Replace("\n", "") == test.output.Replace("\n", ""))
                {
                    testOut.status = "Accepted";
                    subm.correctTestcases++;
                }
                else if (testOut.resultId != 15)
                {
                    testOut.status = "Compilation Error";
                    error = true;
                }
                else
                {
                    testOut.status = "Wrong Answer";
                    wrong = true;
                }
                db.SaveChanges();
            }

            // Check the status of the whole submission (Accepted, wrong answer... etc)
            var sub = db.submission.Where(s => s.id == submissionId).FirstOrDefault();
            if (error)
                sub.status = "Compilation Error";
            else if(wrong)
                sub.status = "Wrong Answer";
            else
                sub.status = "Accepted";

            db.SaveChanges();

            // If accepted, update assignment grade
            if (sub.status == "Accepted")
            {
                int assignmentId = db.problems.Where(a => a.problemId == problemId).Select(a => a.assignmentId).FirstOrDefault();
                UpdateAssignmentGrade(assignmentId, userId);
            }
        }

        public void UpdateAssignmentGrade(int assignmentId, int userId)
        {
            int? teamId = (from team in db.teams
                          where team.assignmentId == assignmentId && team.userId == userId
                          select team.teamId).FirstOrDefault();

            var grade = db.grades.Where(g => g.assignmentId == assignmentId && g.teamId == teamId).FirstOrDefault();
            if (grade == null)
            {
                // create grade
                grade = new grades { assignmentId = assignmentId, teamId = Convert.ToInt32(teamId) };
                db.grades.Add(grade);
            }
            db.SaveChanges();
            var problems = db.problems.Where(p => p.assignmentId == assignmentId).Select(p => p.problem).ToList();
            double finalGrade = 0;
            foreach (var prob in problems)
            {
                int subs = db.submissions.Where(s => s.teamId == teamId && s.submission.problemId == prob.id && s.submission.status == "Accepted").Count();
                if (subs > 0)
                {
                    if (prob.weight != null)
                        finalGrade += (Convert.ToInt32(prob.weight)) / 10;
                }
            }
            grade.grade = Convert.ToInt32(finalGrade);
            db.SaveChanges();
        }

        /*public List<submission> GetSubmissions(int ? id)
        {
            int probId = Convert.ToInt32(id);
            var submissions = (from subm in db.submissions
                           where assign.id.Equals(probId)
                           select problm).ToList();
            return submissions;
        }*/
        

        public submission GetSubmissionById(int? id)
        {
            int submId = Convert.ToInt32(id);
            var submlm = (from subm in db.submission
                          where subm.id.Equals(submId)
                          select subm).FirstOrDefault();
            return submlm;
        }


        public List<submission> GetSubmissionsOfUserOfProblem(int userId, int problemid)
        {
            var ret = (from sub in db.submission
                      join subs in db.submissions on sub.id equals subs.submissionId
                      join tm in db.team on subs.teamId equals tm.id
                      join tms in db.teams on tm.id equals tms.teamId
                      join acc in db.account on tms.userId equals acc.id
                      where acc.id.Equals(userId) && sub.problemId.Equals(problemid)
                      select sub).ToList(); 
            return ret;
        }

        public int GetTeamsBestSubId(int teamid, int problemid)
        {
            var ret = (from sub in db.submission
                      join subs in db.submissions on sub.id equals subs.submissionId
                      where subs.teamId == teamid && sub.problemId == problemid
                      select sub).ToList();
            int maxTest = ret.Max(a => a.correctTestcases);
            return ret.Where(a => a.correctTestcases == maxTest).Select(c => c.id).LastOrDefault();
        }

    }
}