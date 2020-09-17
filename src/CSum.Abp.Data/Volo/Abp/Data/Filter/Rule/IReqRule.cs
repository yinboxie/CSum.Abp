using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Rule
{
    public interface IReqRule
    {
        /// <summary>
        /// 条件间的连接符
        /// </summary>
        ConditionLink Link { get;}
    }
}
