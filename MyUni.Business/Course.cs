using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyUni.Business
{
    public class Course
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Instructor> Instructors { get; set; }
    }
}