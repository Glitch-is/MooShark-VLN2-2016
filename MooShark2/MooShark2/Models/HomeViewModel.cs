using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MooShark2;

namespace MooShark2.Models
{
    public class HomeViewModel
    {
        public account account { get; set; }
        public List<assignment> assignments { get; set; }
        public List<courses> courses { get; set; }
        public Services.GradingService gradingService { get; set; }
    }
}
