using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Data.Filter.Rule;

namespace Volo.Abp.Data.Filter.Group
{
    /// <summary>
    /// 过滤规则组
    /// </summary>
    public class ReqGroup : IReqGroup
    {
        /// <summary>
        /// 获取或设置 条件集合
        /// </summary>
        public ICollection<IReqRule> Rules { get; }

        /// <summary>
        /// 获取或设置 条件间连接符
        /// </summary>
        public ConditionLink Link { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReqGroup() : this(ConditionLink.And)
        { 
        }

        /// <summary>
        /// 使用操作方式初始化一个<see cref="ReqGroup"/>的新实例
        /// </summary>
        /// <param name="link">条件间连接</param>
        public ReqGroup(ConditionLink link)
        {
            Link = link;
            Rules = new List<IReqRule>();
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        public virtual void AddRule(IReqRule rule)
        {
            if (Rules.All(m => !m.Equals(rule)))
            {
                Rules.Add(rule);
            }
        }

        /// <summary>
        /// 添加规则组
        /// </summary>
        public virtual void AddRangeRule(ICollection<IReqRule> rules)
        {
            foreach (var rule in rules)
            {
                AddRule(rule);
            }
        }
    }
}
