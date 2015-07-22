using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyUni.Business
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public Course()
        {
            this.Enrollments = new Collection<Enrollment>();
        }
    }
}