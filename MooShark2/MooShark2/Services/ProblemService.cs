using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooShark2.Services
{
    class ProblemService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        public problem GetProblemById(int ? id)
        {
            int probId = Convert.ToInt32(id);
            var problm = (from prob in db.problem
                         where prob.id.Equals(probId)
                         select prob).FirstOrDefault();
            return problm;
        }


        public testcase GetTestCaseById(int ? id)
        {
            int testId = Convert.ToInt32(id);
            var test = (from tst in db.testcase
                        where tst.id.Equals(testId)
                        select tst).FirstOrDefault();
            return test;
        }

        public testcases GetTestCaseConnectionById(int ? id)
        {
            int testId = Convert.ToInt32(id);
            var tests = (from tsts in db.testcases
                         where tsts.testcaseId.Equals(testId)
                         select tsts).FirstOrDefault();
            return tests;
        }

        public testcaseOutput GetTestCaseOutputById(int ? id)
        {
            int testId = Convert.ToInt32(id);
            var testout = (from tsto in db.testcaseOutput
                           where tsto.testcaseId.Equals(testId)
                           select tsto).FirstOrDefault();
            return testout;
        }

        public problems GetConnectionById(int ? id)
        {
            int probId = Convert.ToInt32(id);
            var problm = (from prob in db.problems
                          where prob.problemId.Equals(probId)
                          select prob).FirstOrDefault();
            return problm;
        }

        public List<problem> GetProblemsOfAssignment(int ? id)
        {
            int probId = Convert.ToInt32(id);
            var problms = (from assign in db.assignment
                           join prob in db.problems on assign.id equals prob.assignmentId
                           join problm in db.problem on prob.problemId equals problm.id
                           where assign.id.Equals(probId)
                           select problm).ToList();
            return problms;
        }

        public List<testcase> getTestCasesOfProblem(int? id)
        {
            int probId = Convert.ToInt32(id);
            var cases = (from prob in db.problem
                         join test in db.testcases on prob.id equals test.problemId
                         join tests in db.testcase on test.testcaseId equals tests.id
                         where prob.id.Equals(probId)
                         select tests).ToList();
            return cases;
        }

        public List<team> GetTeamsThatHaveSubmitted(int problemId)
        {
            var ret = (from sub in db.submission
                       join subs in db.submissions on sub.id equals subs.submissionId
                       join tm in db.team on subs.teamId equals tm.id
                       where sub.problemId.Equals(problemId)
                       select tm).ToList();
            return ret;
        }
    }
}
