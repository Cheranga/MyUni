using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUni.Business
{
    public class Student
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrolledDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
