using MooShark2;
using MooShark2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Services
{
    public class ClarificationService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();
        private Services.UserService usrService = new Services.UserService();

        public List<ClarificationObject> GetClarificationsById(int? id)
        {
            int probId = Convert.ToInt32(id);
            var clarif = (from clars in db.clarifications
                          join clar in db.clarification on clars.clarificationId equals clar.id
                          where clars.problemId.Equals(probId)
                          select clar ).ToList();

            List<ClarificationObject> list = new List<ClarificationObject>();

            foreach(clarification clar in clarif)
            {
                string userN = usrService.getSpecificUser(clar.authorId).name;
                ClarificationQuestion quest = new ClarificationQuestion { userName = userN, question = clar };

                list.AddRange(new List<ClarificationObject>{ new ClarificationObject { question = quest, answers = GetClarificationListAnswersById(clar.id) } });
            }
            return list;
        }

        public List<ClarificationAns> GetClarificationListAnswersById(int? id)
        {
            int clarId = Convert.ToInt32(id);
            var clarAns = (from clarA in db.clarificationAnswer
                          join clar in db.clarification on clarA.clarificationId equals clar.id
                          orderby clarA.posted ascending
                          select clarA).ToList();

            List<ClarificationAns> list = new List<ClarificationAns>();

            foreach(clarificationAnswer clarA in clarAns)
            {
                string userN = usrService.getSpecificUser(clarA.authorId).name;
                ClarificationAns ans = new ClarificationAns { userName = userN, answer = clarA };
                list.AddRange(new List<ClarificationAns> { ans } );
            }
            
            return list;
        }
        // Helper funcitons
        public clarification GetClarificationByClarId(int? id)
        {
            int clarId = Convert.ToInt32(id);
            var clarif = (from clar in db.clarification
                          where clar.id.Equals(clarId)
                          select clar).FirstOrDefault();

            return clarif;
        }

        public clarificationAnswer GetClarificationAnswerById(int? id)
        {
            int clarId = Convert.ToInt32(id);
            var clarAns = (from clarA in db.clarificationAnswer
                           where clarA.clarificationId == clarId
                           select clarA).FirstOrDefault();
            
            return clarAns;
        }
        public List<clarificationAnswer> lisClarAns(int? id)
        {
            int clarId = Convert.ToInt32(id);
            var clarAns = (from clarA in db.clarificationAnswer
                           join clar in db.clarification on clarA.clarificationId equals clar.id
                           select clarA).ToList();
            
            return clarAns;
        }

        // Put question
        public bool AddQuestion(string pId, int userId, string msg)
        {
            int problemId = Convert.ToInt32(pId);
            DateTime localDate = DateTime.Now;

            clarification clar = new clarification { authorId = userId, message = msg, posted = localDate };
            
            db.clarification.Add(clar);
            db.SaveChanges();

            clarifications clars = new clarifications { clarificationId = clar.id, problemId = problemId };

            db.clarifications.Add(clars);
            db.SaveChanges();
            
            return true;
        }

        // Put answer
        public bool AddAnswer(string clarId, int userId, string msg)
        {
            int clarificationId = Convert.ToInt32(clarId);
            DateTime localDate = DateTime.Now;

            clarificationAnswer clar = new clarificationAnswer { authorId = userId, message = msg, posted = localDate, clarificationId = clarificationId};

            db.clarificationAnswer.Add(clar);
            db.SaveChanges();

            return true;
        }
        
        // Delete  
        public bool removeQuestion(int?  clarId)
        {
            int clarificationId = Convert.ToInt32(clarId);
            clarification clar = GetClarificationByClarId(clarificationId);
            List<clarificationAnswer> tempList = lisClarAns(clarificationId);
            var clarS = (from clars in db.clarifications
                          where clars.clarificationId.Equals(clarificationId)
                          select clars).FirstOrDefault();
            try
            {
                foreach(clarificationAnswer ans in tempList)
                {
                    db.clarificationAnswer.Remove(ans);
                    db.SaveChanges();
                }
                db.clarifications.Remove(clarS);
                db.SaveChanges();
                db.clarification.Remove(clar);
                db.SaveChanges();
                
            }
            catch
            {
                return false;
            }
            return true;
        }
        // Delete
        public bool removeAnswer(string id)
        {
            int clarAnsId = Convert.ToInt32(id);
            var clarAns = (from clarA in db.clarificationAnswer
                          where clarA.id.Equals(clarAnsId)
                          select clarA).FirstOrDefault();
     
            try
            {
                db.clarificationAnswer.Remove(clarAns);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public List<clarification> getClarificationsOfProblem(int? id)
        {
            int probId = Convert.ToInt32(id);
            var clarifs = (from prob in db.problem
                         join clar in db.clarifications on prob.id equals clar.problemId
                         join clars in db.clarification on clar.clarificationId equals clars.id
                         where prob.id.Equals(probId)
                         select clars).ToList();
            return clarifs;
        }

    }
}