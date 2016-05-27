using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooShark2.Models
{
    class AdminViewModel
    {
    }

    public class ManageUsersViewModel
    {
        public List<account> users { get; set; }
        public account currentUser { get; set; }
    }
    public class ManageCoursesViewModel
    {
        public List<courses> courses { get; set; }
        public List<account> teachers { get; set; }
        public courses currentUser { get; set; }
    }
    public class CourseUsersViewModel
    {
        public int courseId { get; set; }
        public List<account> enrolled { get; set; }
        public List<account> notEnrolled { get; set; }
    }
    public class AdminPanelViewModel
    {
        public List<account> users   { get; set; }
        public List<courses> courses { get; set; }
    }
}
