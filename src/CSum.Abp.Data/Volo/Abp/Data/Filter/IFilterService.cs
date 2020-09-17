using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data.Filter.Translate;

namespace Volo.Abp.Data.Filter
{
    public interface IFilterService
    {
        /// <summary>
        /// 转换过滤模型
        /// </summary>
        /// <param name="model">过滤模型</param>
        /// <returns>返回过滤规则条件</returns>
        FilteringCondition Translate(IFilterModel model);

        /// <summary>
        /// 转换过滤模型
        /// </summary>
        /// <param name="model">过滤模型</param>
        /// <param name="translator">翻译器</param>
        /// <returns>返回过滤规则条件</returns>
        FilteringCondition Translate(IFilterModel model, ITranslator translator);
    }
}
