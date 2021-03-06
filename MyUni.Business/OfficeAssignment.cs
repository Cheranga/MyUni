using System.ComponentModel.DataAnnotations;

namespace MyUni.Business
{
    public class OfficeAssignment
    {
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }

        [Display(Name = "Office Location")]
        public string Location { get; set; }
    }
}