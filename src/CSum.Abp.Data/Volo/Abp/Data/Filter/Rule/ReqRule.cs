using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Volo.Abp.Data.Filter.Rule
{
    /// <summary>
    /// 过滤规则
    /// </summary>
    public class ReqRule : IReqRule
    {
        /// <summary>
        /// 获取或设置 属性名称
        /// </summary>
        public string Field { get; protected set; }

        /// <summary>
        /// 获取或设置 属性值
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// 获取或设置 操作类型
        /// </summary>
        public FilterOperate Operate { get; protected set; }

        /// <summary>
        /// 获取或设置 条件连接
        /// </summary>
        public ConditionLink Link { get; protected set; }

        /// <summary>
        /// 转换类型，字段类型进行转换，例如枚举值转成int
        /// </summary>
        public string ConversionType { get; protected set; }

        /// <summary>
        /// 使用指定数据名称，数据值及操作方式初始化一个<see cref="ReqRule"/>的新实例
        /// </summary>
        /// <param name="field">数据名称</param>
        /// <param name="value">数据值</param>
        /// <param name="operate">操作方式</param>
        public ReqRule(string field, object value, FilterOperate operate, ConditionLink link)
        {
            Field = field;
            Value = value;
            Operate = operate;
            Link = link;
        }

        /// <summary>
        /// 设置转换类型
        /// </summary>
        /// <param name="conversionType"></param>
        /// <param name="conversionTypeName"></param>
        public void SetConversionType([NotNull]Type conversionType, [NotNull]string conversionTypeName)
        {
            ConversionType = conversionTypeName;
            Value = Convert.ChangeType(Value, conversionType, CultureInfo.InvariantCulture);
        }

        #region Overrides of Object

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ReqRule rule))
            {
                return false;
            }
            return rule.Field == Field && rule.Value == Value && rule.Operate == Operate && rule.Link == Link;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return $"{Field}-{Value}-{Operate}-{Link}".GetHashCode();
        }

        #endregion
    }
}
