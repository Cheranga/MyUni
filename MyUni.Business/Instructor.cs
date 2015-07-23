using System;
using System.Collections.Generic;

namespace MyUni.Business
{
    public class Instructor
    {
        public int Id { get; set; }
        public int? OfficeAssignmentId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.FirstName, this.LastName); }
        }

        public DateTime HireDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public virtual OfficeAssignment OfficeAssignment { get; set; }

        public virtual ICollection<Department> DepartmentsWhichAdministers { get; set; }
    }
}