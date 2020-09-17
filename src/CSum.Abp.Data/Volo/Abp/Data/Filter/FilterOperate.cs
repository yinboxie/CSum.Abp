using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 过滤操作
    /// </summary>
    public enum FilterOperate
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        [Operate("=")]
        Equal = 1,

        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        [Operate("!=")]
        NotEqual = 2,

        /// <summary>
        /// 小于
        /// </summary>
        [Description("小于")]
        [Operate("<")]
        Less = 3,

        /// <summary>
        /// 小于或等于
        /// </summary>
        [Description("小于等于")]
        [Operate("<=")]
        LessOrEqual = 4,

        /// <summary>
        /// 大于
        /// </summary>
        [Description("大于")]
        [Operate(">")]
        Greater = 5,

        /// <summary>
        /// 大于或等于
        /// </summary>
        [Description("大于等于")]
        [Operate(">=")]
        GreaterOrEqual = 6,

        /// <summary>
        /// 以……开始
        /// </summary>
        [Description("开始于")]
        [Operate("like")]
        StartsWith = 7,

        /// <summary>
        /// 以……结束
        /// </summary>
        [Description("结束于")]
        [Operate("like")]
        EndsWith = 8,

        /// <summary>
        /// 字符串的包含（相似）
        /// </summary>
        [Description("包含")]
        [Operate("like")]
        Contains = 9,
    }
}
