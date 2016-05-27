using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Models
{
    public class ProblemsViewModel
    {
        public IEnumerable<problem> probs { get; set; }
        public IEnumerable<account> teamMembers { get; set; }
        public int cid { get; set; }
        public int aid { get; set; }
        public account currentUser { get; set; }
    }

    public class ProblemViewModel
    {
        public problem prob { get; set; }
        public account currentUser { get; set; }
    }
    
    public class ScoreboardViewModel
    {
        public assignment assignment { get; set; }
        public int probs { get; set; }
        public List<ProblemSuccessHolder> tableData { get; set; }
    }

    public class ProblemSuccessHolder
    {
        public string name { get; set; }
        public List<bool> success { get; set; }
    }
    public class EditProblemViewModel
    {
        public problem prob { get; set; }
        public List<testcase> cases { get; set; }

    }
}