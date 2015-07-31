using System.Collections.Generic;
using System.ComponentModel;

namespace MyUni.Web.ViewModels
{
    public class InstructorCreateViewModel : InstructorViewModel
    {
        [DisplayName("Location")]
        public string OfficeLocation { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class CustomerList
    {
        public string ListName { get; set; }
        public IList<Customer> Customers { get; set; }
    }
}