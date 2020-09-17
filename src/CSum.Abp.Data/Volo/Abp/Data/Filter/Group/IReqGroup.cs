using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data.Filter.Rule;

namespace Volo.Abp.Data.Filter.Group
{
    public interface IReqGroup
    {
        /// <summary>
        /// 获取或设置 条件集合
        /// </summary>
        ICollection<IReqRule> Rules { get; }

        /// <summary>
        /// 获取或设置 条件间连接符
        /// </summary>
        ConditionLink Link { get;}

        /// <summary>
        /// 添加过滤规则
        /// </summary>
        /// <param name="rule"></param>
        void AddRule(IReqRule rule);

        /// <summary>
        /// 添加过滤规则集合
        /// </summary>
        /// <param name="rules"></param>
        void AddRangeRule(ICollection<IReqRule> rules);
    }
}
