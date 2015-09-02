using System;
using System.ComponentModel.DataAnnotations;

namespace MyUni.Web.ViewModels.Student
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Enrolled Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime EnrolledDate { get; set; }

        public double EnrolledDateForScript
        {
            get
            {
                return this.EnrolledDate.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", this.FirstName, this.LastName); }
        }
        
    }
}