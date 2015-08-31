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

        public IEnumerable<DataTableColumnInfo> OrderedColumns { get; set; }


        public int PageNumber
        {
            get { return (this.Length == 0) ? 0 : (this.Start/this.Length); }
        }

        public string OrderByExpression
        {
            get
            {
                return OrderedColumns == null
                    ? string.Empty
                    : string.Join(",", this.OrderedColumns.Select(x => string.Format("{0} {1}", x.Field, x.ColumnOrder)));
            }
        }

        //
        // TODO: The where condition for filtering
        //


        public DataTableInfo()
        {
            this.OrderedColumns = new List<DataTableColumnInfo>();
        }
    }
}