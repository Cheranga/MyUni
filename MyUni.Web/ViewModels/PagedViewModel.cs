using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MyUni.Web.ViewModels
{
    //public class StudentListViewModel
    //{
    //    public string Search { get; set; }

    //    private IEnumerable<Business.Student> Students { get; set; }

    //    public int CurrentPage { get; set; }

    //    private int pageSize = 0;
    //    public int PageSize
    //    {
    //        get { return this.pageSize; }
    //        set { this.pageSize = value; }
    //    }

    //    public int TotalPages
    //    {
    //        get
    //        {
    //            if (this.Students == null || !this.Students.Any())
    //            {
    //                return 0;
    //            }

    //            var totalPages = this.Students.Count()/this.pageSize;
    //            totalPages = totalPages == 0 ? 1 : totalPages;

    //            return totalPages;
    //        }

    //        private set { ; }
    //    }

    //    public IEnumerable<Business.Student> 

    //    public StudentListViewModel(IEnumerable<Business.Student> students, int currentPage = 1 )
    //    {
    //        //
    //        // Setting the student collection
    //        //
    //        this.Students = students ?? new List<Business.Student>();
    //        //
    //        // Setting the page size from the configuration file
    //        //
    //        var pageSizeSetting = ConfigurationManager.AppSettings.Get("pageSize");
    //        int.TryParse(pageSizeSetting, out this.pageSize);
    //        pageSize = pageSize <= 0 ? 10 : pageSize;
    //    }
    //}


    public class PagedViewModel<T> where T : class
    {
        public string Search { get; set; }
        public bool FromSearch { get; set; }

        private int pageSize = 10;
        public int PageSize
        {
            get { return this.pageSize; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("pageSize cannot be less than or equal to zero");
                }

                this.pageSize = value;

                CalculateTotalPageCount();
                FilterCollection();
            }
        }

        private int currentPage = 1;

        public int CurrentPage
        {
            get { return this.currentPage; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Current page cannot be zero or less");
                }
            }
        }


        private int totalPages = 0;

        private readonly IEnumerable<T> collection = null;
        private IEnumerable<T> filteredCollection = null;

        public IEnumerable<T> FilteredCollection
        {
            get { return this.filteredCollection; }
        }

        public int TotalPages
        {
            get { return this.totalPages; }
        }


        public PagedViewModel(IEnumerable<T> collection, int? pageSize = null, int currentPage = 1 )
        {
            this.collection = collection ?? new List<T>();
            this.filteredCollection = new List<T>();

            this.currentPage = currentPage <= 0 ? 1 : currentPage;

            if (pageSize.HasValue)
            {
                this.PageSize = pageSize.Value <= 0 ? 10 : pageSize.Value;
            }
            else
            {
                var pgSize = 0;
                int.TryParse(ConfigurationManager.AppSettings.Get("pageSize"), out pgSize);
                this.PageSize = pgSize <= 0 ? 10 : pgSize;
            }
            

            CalculateTotalPageCount();
            FilterCollection();
        }

        private void CalculateTotalPageCount()
        {
            var totalPageCount = (this.collection.Count() / this.PageSize) == 0
               ? 1
               : this.collection.Count() / this.PageSize;

            this.totalPages = totalPageCount;
        }

        public void FilterCollection()
        {
            this.filteredCollection = this.collection.Skip((this.currentPage - 1)*this.pageSize).Take(this.pageSize);
        }
    }
}