using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 条件之间的连接符号
    /// </summary>
    public enum ConditionLink
    {
        /// <summary>
        /// 并且
        /// </summary>
        [Description("并且")]
        And = 1,

        /// <summary>
        /// 或者
        /// </summary>
        [Description("或者")]
        Or = 2,
    }
}
