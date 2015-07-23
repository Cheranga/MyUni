using System;
using System.Collections.Generic;

namespace MyUni.Business
{
    public class Department
    {
        public int Id { get; set; }
        public int? AdministratorId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }

        public virtual Instructor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}