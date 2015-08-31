using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MyUni.Web.Infrastructure
{
    public class DataTableModelBinder : DefaultModelBinder
    {
        private const string ORDER_COLUMN_FORMAT = @"order\[[0-9]+\]\[column\]$";
        private const string COLUMN_DATA_FORMAT = "columns[{0}][data]";
        private const string ORDER_DIR_FORMAT = "order[{0}][dir]";
        private const string DRAW = "draw";
        private const string START = "start";
        private const string LENGTH = "length";
        private const string SEARCH = "search[value]";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                //
                // Bootstrap datatable will transfer the data in the query string
                //
                var queryStringData = controllerContext.HttpContext.Request.QueryString;

                //
                // Get the ordered columns, this is transferred in the format "order[<index>][column], where "index" is a number
                //
                var queryStringDataList = queryStringData.AllKeys;
                var regEx = new Regex(ORDER_COLUMN_FORMAT);

                //
                // We need to transform the posted data, as a collection of "DataTableColumnInfo" objects.
                //
                var orderedColumns = queryStringDataList.ToList().FindAll(regEx.IsMatch).Select(x =>
                {
                    var colIndexData = queryStringData[x];
                    var orderIndexData = Regex.Replace(x, @"[^\d]", "");

                    var colIndex = 0;
                    var orderIndex = 0;

                    if (int.TryParse(colIndexData, out colIndex) && int.TryParse(orderIndexData, out orderIndex))
                    {

                        return new DataTableColumnInfo
                        {
                            Field = queryStringData[string.Format(COLUMN_DATA_FORMAT, colIndex)],
                            ColumnOrder = queryStringData[string.Format(ORDER_DIR_FORMAT, orderIndex)] == "asc" ? ColumnSortOrder.Asc : ColumnSortOrder.Desc
                        };
                    }
                    return null;
                }).Where(x => x != null).ToList();
                //
                // Along with the ordered columns, get the other required data. Such as "draw, start, length, search"
                //
                var draw = GetValue<int>(queryStringData, "draw");
                var start = GetValue<int>(queryStringData, "start");
                var length = GetValue<int>(queryStringData, "length");
                var search = GetValue<string>(queryStringData, "search[value]");

                return new DataTableInfo
                {
                    Draw = draw,
                    Start = start,
                    Length = length,
                    Search = search,
                    OrderedColumns = orderedColumns
                };
            }
            catch (Exception exception)
            {
                //
                // Pass the exception back to the application
                //
                throw;
            }
            
        }

        private T GetValue<T>(NameValueCollection queryStringNameValueCollection, string name)
        {
            if (queryStringNameValueCollection == null || string.IsNullOrEmpty(name))
            {
                return default(T);
            }

            var value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(queryStringNameValueCollection[name]);
            return value;
        }
    }
}