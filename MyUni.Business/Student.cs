using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUni.Business
{
    public class Student : IModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrolled Date")]
        public DateTime EnrolledDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.FirstName, this.LastName); }
        }

    }
}
