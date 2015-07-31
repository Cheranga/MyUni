using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyUni.Business;

namespace MyUni.Web.ViewModels
{
    public class InstructorListViewModel
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}