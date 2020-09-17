using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Volo.Abp.Data.Filter.Rule
{
    /// <summary>
    /// 多个字段条件，例如:关键词查询
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReqMultiFieldAttribute : Attribute, IReqFieldAttribute
    {
        /// <summary>
        /// 字段名数组
        /// </summary>
        public string[] Fields { get; set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public FilterOperate Op { get; protected set; }

        /// <summary>
        /// 内部条件连接
        /// </summary>
        public ConditionLink InnerLink { get; protected set; }

        /// <summary>
        /// 外部条件连接
        /// </summary>
        public ConditionLink OuterLink { get; protected set; }

        public ReqMultiFieldAttribute(string[] fields)
            :this(fields,FilterOperate.Contains)
        { 
        }

        public ReqMultiFieldAttribute(string[] fields, FilterOperate op)
            : this(fields, op, ConditionLink.Or)
        {
        }

        public ReqMultiFieldAttribute(string[] fields, FilterOperate op, ConditionLink innerLink)
            : this(fields, op, innerLink, ConditionLink.And)
        {
        }

        public ReqMultiFieldAttribute(string[] fields,
           FilterOperate op,
           ConditionLink innerLink,
           ConditionLink outerLink)
        {
            Fields = fields;
            Op = op;
            InnerLink = innerLink;
            OuterLink = outerLink;
        }

        public virtual IReqRule ParseRule([NotNull]string fieldName, [NotNull]object value)
        {
            var mRule = new ReqMultiRule(OuterLink);
            foreach (var field in Fields)
            {
                mRule.AddRule(new ReqRule(field, value, Op, InnerLink));
            }
            return mRule;
        }
    }
}
