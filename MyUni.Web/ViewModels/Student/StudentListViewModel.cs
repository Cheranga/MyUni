using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyUni.Web.ViewModels.Student
{
    public class StudentListViewModel
    {
        public string Search { get; set; }

        public IEnumerable<Business.Student> Students { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public StudentListViewModel()
        {
            this.Students = new List<Business.Student>();
        }
    }
}