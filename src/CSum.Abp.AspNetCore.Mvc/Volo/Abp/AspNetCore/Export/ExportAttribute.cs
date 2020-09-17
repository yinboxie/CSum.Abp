using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AspNetCore.Export
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExportAttribute : Attribute, IExportData
    {
        /// <summary>
        /// 导出模型类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 导出模板路径
        /// </summary>
        public string TemplatePath { get; set; }
        /// <summary>
        /// 导出文件名
        /// </summary>
        public string FileName { get; set; }
    }
}
