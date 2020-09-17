using JetBrains.Annotations;
using System;

namespace Volo.Abp.Data.Filter.Rule
{
    public class ReqBetweenRule : IReqRule
    {
        /// <summary>
        /// 获取或设置 属性名称
        /// </summary>
        public string Field { get; protected set; }

        /// <summary>
        /// 获取或设置 开始值
        /// </summary>
        public object StartValue { get; protected set; }

        /// <summary>
        /// 获取或设置 结束值
        /// </summary>
        public object EndValue { get; protected set; }

        /// <summary>
        /// 获取或设置 条件连接
        /// </summary>
        public ConditionLink Link { get; protected set; }

        public ReqBetweenRule(string field, [NotNull]object startValue, [NotNull]object endValue, ConditionLink link)
        {
            Field = field;
            StartValue = startValue;
            EndValue = endValue;
            Link = link;
        }

        #region Overrides of Object

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ReqBetweenRule rule))
            {
                return false;
            }
            return rule.Field == Field && rule.StartValue == StartValue && rule.EndValue == EndValue && rule.Link == Link;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return $"{Field}-{StartValue}-{EndValue}-{Link}".GetHashCode();
        }

        #endregion
    }
}
