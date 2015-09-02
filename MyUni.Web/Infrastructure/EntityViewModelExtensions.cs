using System;
using System.Collections.Generic;
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
                EnrolledDate = studentViewModel.EnrolledDate
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
                EnrolledDate = student.EnrolledDate
            };

            return viewModel;
        }
    }
}