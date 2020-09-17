using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 过滤的条件
    /// </summary>
    public class FilteringCondition
    {
        /// <summary>
        /// 原始条件对象
        /// </summary>
        public IFilterModel OriginalModel { get; }

        /// <summary>
        /// 过滤条件文本
        /// </summary>
        public string ConditionalText { get; protected set; }

        /// <summary>
        /// Sql文本
        /// </summary>
        public string SqlCommandText { get { return GetSqlText(); } }

        /// <summary>
        /// 过滤的参数
        /// </summary>
        public IList<FilteringParam> FilteringParams { get; protected set; }

        /// <summary>
        /// 额外的属性, 未标注<see cref="IReqFieldAttribute"/>的字段
        /// </summary>
        public Dictionary<string, object> ExtraProperties { get; protected set; }


        public FilteringCondition(IFilterModel model)
        {
            OriginalModel = model;
            FilteringParams = new List<FilteringParam>();
            ExtraProperties = new Dictionary<string, object>();
        }

        /// <summary>
        /// 获取未转换的额外属性值内容
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetExtraPropertyValue(string propertyName)
        {
            return ExtraProperties.GetOrDefault(propertyName);
        }

        /// <summary>
        /// 获取属性原始值内容
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetOrignalPropertyValue(string propertyName)
        {
            return OriginalModel.GetType().GetProperty(propertyName).GetValue(OriginalModel);
        }

        /// <summary>
        /// 获取动态参数对象
        /// </summary>
        /// <param name="isIncludeExtra">是否包含额外的属性</param>
        /// <returns></returns>
        public object GetDynamicParams(bool isIncludeExtra= true)
        {
            var pdic = new Dictionary<string, object>();
            if (isIncludeExtra)
            {
                foreach (var item in ExtraProperties)
                {
                    pdic.Add(item.Key, item.Value);
                }
            }

            foreach (var item in FilteringParams)
            {
                //去除参数的符号
                pdic.Add(item.Name.Substring(1), item.Value);
            }
            return pdic.ToDynamic();
        }

        /// <summary>
        /// 设置额外属性
        /// </summary>
        /// <param name="keyValuePairs"></param>
        internal void SetExtraProperties([NotNull]Dictionary<string, object> keyValuePairs)
        {
            ExtraProperties = keyValuePairs;
        }

        /// <summary>
        /// 设置过滤的参数集
        /// </summary>
        /// <param name="filteringParams"></param>
        internal void SetFilteringParams([NotNull]IEnumerable<FilteringParam> filteringParams)
        {
            FilteringParams = filteringParams.ToList();
        }

        /// <summary>
        /// 设置条件文件
        /// </summary>
        /// <param name="conditionalText"></param>
        internal void SetConditionalText([NotNull]string conditionalText)
        {
            ConditionalText = conditionalText;
        }

        private string GetSqlText()
        {
            StringBuilder builder = new StringBuilder(ConditionalText);
            foreach (var item in FilteringParams)
            {
                builder.Replace(item.Name, item.Value.ToString());
            }
            return builder.ToString();
        }
    }
}
