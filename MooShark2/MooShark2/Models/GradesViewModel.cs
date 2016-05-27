using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MooShark2.Services;

namespace MooShark2.Models
{
    public class GradesViewModel
    {
        public int assignmentId { get; set; }
        public IEnumerable<grades> grades { get; set; }
        public GradingService gradingService { get; set; }
        public AssignmentService assignmentService { get; set; }
    }
}