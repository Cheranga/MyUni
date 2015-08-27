using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyUni.Web.Infrastructure
{
    public class DataTableInfo
    {
        public int Draw { get; set; }

        public int Start { get; set; }
        public int Length { get; set; }


        public string Search { get; set; }

        public IEnumerable<OrderedColumn> OrderedColumns { get; set; }


        public int PageNumber
        {
            get { return (this.Length == 0) ? 0 : (this.Start/this.Length); }
        }


        public DataTableInfo()
        {
            this.OrderedColumns = new List<OrderedColumn>();
        }
    }

    public class DataTableInfo<T> where T:class 
    {
        public int Draw { get; set; }

        public int Start { get; set; }
        public int Length { get; set; }


        public string Search { get; set; }

        public IEnumerable<Func<T, object>> OrderByExpressions { get; set; }

        public IEnumerable<OrderedColumn> OrderedColumns { get; set; }


        public int PageNumber
        {
            get { return (this.Length == 0) ? 0 : (this.Start / this.Length); }
        }


        public DataTableInfo()
        {
            this.OrderedColumns = new List<OrderedColumn>();
            this.OrderByExpressions = new List<Func<T, object>>();
        }
    }

    public enum ColumnOrder
    {
        Asc,
        Desc
    }

    public class OrderedColumn
    {
        public string Field { get; set; }
        public ColumnOrder ColumnOrder { get; set; }
    }
}