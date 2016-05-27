using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Models
{
    public class ClarificationsViewModel
    {
        public int pId { get; set; }
        public List<ClarificationObject> clarification { get; set; }

        public account currentUser { get; set; }
    }

    public class ClarificationsEmptyViewModel
    {
        public int pId { get; set; }

        public account currentUser { get; set; }
    }
}