using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyUni.Business
{
    public class Course : IModel
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public Department Department { get; set; }
        public ICollection<Instructor> Instructors { get; set; }
    }
}