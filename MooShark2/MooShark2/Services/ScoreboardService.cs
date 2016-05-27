using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooShark2.Services
{
    class ScoreboardService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        public List<team> GetTeamsOfAssignment(int assId)
        {
            var ret = (from tms in db.teams
                      join tm in db.team on tms.teamId equals tm.id
                      where tms.assignmentId == assId
                      select tm).ToList();
            return ret;
        }

        public bool CheckIfDone(int probId, int teamId)
        {
            var query = (from submission in db.submission
                         join submissionRelation in db.submissions on submission.id equals submissionRelation.submissionId
                         where submissionRelation.teamId == teamId && submission.status == "Accepted" && submission.problemId == probId
                         select submission).Count();

            return query > 0;
        }

        

    }
}
