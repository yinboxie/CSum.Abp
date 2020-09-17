using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Group
{
    /// <summary>
    /// 过滤字段分组特性约束
    /// </summary>
    public interface IReqGroupAttribute
    {
        /// <summary>
        ///  分组序号
        /// </summary>
        int SortCode { get; set; }

        /// <summary>
        ///  分组之间的条件连接符
        /// </summary>
        ConditionLink Link { get; set; }
    }
}
