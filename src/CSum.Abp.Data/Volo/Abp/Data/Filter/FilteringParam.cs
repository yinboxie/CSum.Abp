using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 过滤的参数
    /// </summary>
    public class FilteringParam
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }

        public FilteringParam(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
