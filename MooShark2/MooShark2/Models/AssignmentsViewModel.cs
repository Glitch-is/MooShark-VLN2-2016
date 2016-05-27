using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MooShark2.Services;

namespace MooShark2.Models
{
    public class AssignmentsViewModel
    {
        
        public int courseId { get; set; }
        public List<assignment> assignments { get; set; }
        public account user { get; set; }
        public GradingService gradingService { get; set; }
    }
}
