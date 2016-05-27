using MooShark2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Models
{
    public class ClarificationObject
    {
        public ClarificationQuestion question;

        public IEnumerable<ClarificationAns> answers;
    }
    public class ClarificationQuestion
    {
        public string userName;

        public clarification question;
    }

    public class ClarificationAns
    {
        public string userName;

        public clarificationAnswer answer;
    }
}