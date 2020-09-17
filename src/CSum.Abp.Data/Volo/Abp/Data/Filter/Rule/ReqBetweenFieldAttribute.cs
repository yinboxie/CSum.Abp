using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Rule
{
    /// <summary>
    /// 范围字段条件，例如:时间范围
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReqBetweenFieldAttribute: Attribute, IReqFieldAttribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 条件连接
        /// </summary>
        public ConditionLink Link { get; protected set; }

        /// <summary>
        /// 分割符
        /// </summary>
        public char Split { get; set; }

        public ReqBetweenFieldAttribute(string fieldName)
            :this(fieldName, ConditionLink.And)
        { 
        }

        public ReqBetweenFieldAttribute(string fieldName, ConditionLink link)
            : this(fieldName, link, '~')
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="split">默认符号 ~</param>
        public ReqBetweenFieldAttribute(string fieldName = "", ConditionLink link = ConditionLink.And, char split = '~')
        {
            FieldName = fieldName;
            Link = link;
            Split = split;
        }

        public virtual IReqRule ParseRule([NotNull]string fieldName, [NotNull]object value)
        {
            var arr = value.ToString().Split(Split);
            if (arr.Length != 2 || (arr[0].IsNullOrWhiteSpace() || arr[1].IsNullOrWhiteSpace()))
            {
                throw new Exception($"值内容不合法，过滤规则({typeof(ReqBetweenFieldAttribute).FullName})解析失败");
            }
            return new ReqBetweenRule(FieldName.IsNullOrWhiteSpace() ? fieldName : FieldName, arr[0], arr[1], Link);
        }
    }
}
