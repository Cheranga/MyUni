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
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var queryStringData = controllerContext.HttpContext.Request.QueryString;

            
            

            var queryStringDataList = queryStringData.AllKeys;
            var regEx = new Regex(@"order\[[0-9]+\]\[column\]$");
            //var orderedKeys = queryStringDataList.ToList().FindAll(regEx.IsMatch).Select(x => Regex.Replace(x, @"[^\d]", ""));

            var orderedColumns = queryStringDataList.ToList().FindAll(regEx.IsMatch).Select(x =>
            {
                var colIndexData = queryStringData[x];
                var orderIndexData = Regex.Replace(x, @"[^\d]", "");

                var colIndex = 0;
                var orderIndex = 0;

                if (int.TryParse(colIndexData, out colIndex) && int.TryParse(orderIndexData, out orderIndex))
                {

                    return new OrderedColumn
                    {
                        Field = queryStringData[string.Format("columns[{0}][data]", colIndex)],
                        ColumnOrder = queryStringData[string.Format("order[{0}][dir]", orderIndex)] == "asc" ? ColumnOrder.Asc : ColumnOrder.Desc
                    };
                }
                return null;
                //return string.IsNullOrEmpty(value) ? string.Empty : x;
            }).Where(x => x != null).ToList();
            //
            // Get the required values
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