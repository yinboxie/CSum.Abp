using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Volo.Abp.Data.Filter.Rule
{
    public class ReqMultiRule : IReqRule
    {
        /// <summary>
        /// 获取或设置 条件连接
        /// </summary>
        public ConditionLink Link { get; protected set; }

        /// <summary>
        /// 规则
        /// </summary>
        public ICollection<IReqRule> Rules { get; }

        public ReqMultiRule(ConditionLink link)
        {
            Link = link;
            Rules = new List<IReqRule>();
        }

        /// <summary>
        /// 添加规则
        /// </summary>
        public void AddRule(IReqRule rule)
        {
            if (Rules.All(m => !m.Equals(rule)))
            {
                Rules.Add(rule);
            }
        }
    }
}
