using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MyUni.Business;
using MyUni.Web.ViewModels.Student;

namespace MyUni.Web.Infrastructure
{
    public static class EntityViewModelExtensions
    {
        public static Student ToEntity(this StudentViewModel studentViewModel)
        {
            var student = new Student
            {
                Id = studentViewModel.Id,
                FirstName = studentViewModel.FirstName,
                LastName = studentViewModel.LastName,
                EnrolledDate = studentViewModel.EnrolledDate,
                Enrollments = studentViewModel.Enrollments == null ? new Collection<Enrollment>() :
                    new Collection<Enrollment>(studentViewModel.Enrollments.Select(x => x.ToEntity()).ToList())
            };

            return student;
        }

        public static StudentViewModel ToViewModel(this Student student)
        {
            var viewModel = new StudentViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                EnrolledDate = student.EnrolledDate,
                Enrollments = student.Enrollments == null ? new List<EnrollmentViewModel>() : student.Enrollments.Select(x => x.ToViewModel())
            };

            return viewModel;
        }

        public static Enrollment ToEntity(this EnrollmentViewModel enrollmentViewModel)
        {
            var enrollment = new Enrollment
            {
                Id = enrollmentViewModel.Id,
                CourseId = enrollmentViewModel.CourseId,
                StudentId = enrollmentViewModel.StudentId,
                Grade = enrollmentViewModel.Grade
            };

            return enrollment;
        }

        public static EnrollmentViewModel ToViewModel(this Enrollment entity)
        {
            var viewModel = new EnrollmentViewModel
            {
                Id = entity.Id,
                CourseId = entity.Id,
                CourseTitle = entity.Course.Title,
                StudentId = entity.CourseId,
                Grade = entity.Grade.HasValue ? entity.Grade.Value : Grade.NA
            };

            return viewModel;
        }
    }
}