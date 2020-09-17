using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Translate
{
    /// <summary>
    /// 过滤规则翻译之后的结果
    /// </summary>
    public class TranslateResult
    {
        /// <summary>
        /// 条件文本
        /// </summary>
        public string ConditionalText { get; protected set; }

        /// <summary>
        /// 过滤参数集
        /// </summary>
        public IEnumerable<FilteringParam> FilteringParams { get; protected set; }

        public TranslateResult(string conditionalText, IEnumerable<FilteringParam> filteringParams)
        {
            ConditionalText = conditionalText;
            FilteringParams = filteringParams;
        }
    }
}
