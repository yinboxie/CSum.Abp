using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Data
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// DataTable转换成实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            var jsonstr = JsonConvert.SerializeObject(dt);
            return JsonConvert.DeserializeObject<List<T>>(jsonstr);
        }
        /// <summary>
        ///  DataTable转换成实体列表
        /// </summary>
        /// <typeparam name="T">实体 T </typeparam>
        /// <param name="dt">datatable</param>
        /// <returns></returns>
        public static List<T> ToListEx<T>(this DataTable dt)
            where T : class
        {
            if (typeof(T).GetConstructors().Count(p=> p.GetParameters().Count() == 0) == 0)
            {
                throw new Exception($"类型({typeof(T).Name})不具有无参构造函数");
            }
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                var model = Activator.CreateInstance<T>();

                foreach (DataColumn dc in dr.Table.Columns)
                {
                    var drValue = dr[dc.ColumnName];
                    var pi = model.GetType().GetProperty(dc.ColumnName);

                    if (pi != null && pi.CanWrite && drValue != null && !Convert.IsDBNull(drValue))
                        pi.SetValue(model, drValue.To(pi.PropertyType), null);
                }
                list.Add(model);
            }
            return list;  
        }
    }
}
