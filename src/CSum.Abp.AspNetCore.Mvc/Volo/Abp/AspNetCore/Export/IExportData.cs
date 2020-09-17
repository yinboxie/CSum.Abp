using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AspNetCore.Export
{
    public interface IExportData
    {
        /// <summary>
        /// 导出模型类型
        /// </summary>
        Type Type { get; set; }
        /// <summary>
        /// 导出模板路径
        /// </summary>
        string TemplatePath { get; set; }
        /// <summary>
        /// 导出文件名
        /// </summary>
        string FileName { get; set; }
    }
}
