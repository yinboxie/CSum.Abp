using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 操作符号特性标注，例如 <、 <=、 like
    /// </summary>
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
    public class OperateAttribute : Attribute
    {
        /// <summary>
        /// 符号
        /// </summary>
        public string Code { get; set; }

        public OperateAttribute(string code)
        {
            Code = code;
        }
    }
}
