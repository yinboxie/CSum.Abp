using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Domain.Entities
{
    /// <summary>
    /// 可排序实体接口,继承该接口后，GetList查询会使用SortCode值进行排序
    /// </summary>
    public interface ISortable
    {
        /// <summary>
        /// 排序码，数字越小越靠前
        /// </summary>
        int SortCode { get; set; }
    }
}
