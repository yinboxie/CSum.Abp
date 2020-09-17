using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data.Filter.Group;

namespace Volo.Abp.Data.Filter.Translate
{
    /// <summary>
    /// 过滤规则翻译上下文
    /// </summary>
    public class TranslateContext
    {
        /// <summary>
        /// Db 参数前缀
        /// </summary>
        public string DbParameterPrefix { get; set; }

        /// <summary>
        /// 过滤规则组
        /// </summary>
        public ReqGroupList FilterGroup { get; set; }
    }
}
