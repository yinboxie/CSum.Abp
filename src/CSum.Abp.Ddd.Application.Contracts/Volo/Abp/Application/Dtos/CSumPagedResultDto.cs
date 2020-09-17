using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    /// <summary>
    /// 分页结果(CSum前端框架特定)
    /// </summary>
    public class CSumPagedResultDto
    {
        /// <summary>
        /// 行内容结果集
        /// </summary>
        public object rows { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long total { get; set; }
    }
}
