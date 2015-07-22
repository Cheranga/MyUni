using System.Collections.Generic;

namespace MyUni.Business
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}