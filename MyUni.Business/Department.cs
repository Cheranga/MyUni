using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyUni.Business
{
    public class Department : IModel
    {
        public int Id { get; set; }

        [DisplayName("Administrator")]
        public int? AdministratorId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Started On")]
        public DateTime StartDate { get; set; }

        public Instructor Administrator { get; set; }
        public ICollection<Course> Courses { get; set; }

        public byte[] RowVersion { get; set; }

    }
}