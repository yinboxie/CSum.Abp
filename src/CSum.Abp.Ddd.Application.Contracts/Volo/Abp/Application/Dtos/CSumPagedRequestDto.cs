using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    public class CSumPagedRequestDto
    {
        /// <summary>
        /// 每页行数,为0是查最多行数
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; } = -1;

        /// <summary>
        /// 排序列
        /// </summary>
        public string sidx { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public string sord { get; set; } = "asc";
    }
}
