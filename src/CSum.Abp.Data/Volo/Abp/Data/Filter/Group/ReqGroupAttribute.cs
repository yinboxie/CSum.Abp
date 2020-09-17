using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Group
{
    /// <summary>
    /// 条件分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReqGroupAttribute : Attribute, IReqGroupAttribute
    {
        /// <summary>
        ///  分组序号
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        ///  分组之间的条件连接符
        /// </summary>
        public ConditionLink Link { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sortCode"></param>
        /// <param name="link"></param>
        public ReqGroupAttribute(int sortCode, ConditionLink link = ConditionLink.And)
        {
            SortCode = sortCode;
            Link = link;
        }
    }
}
