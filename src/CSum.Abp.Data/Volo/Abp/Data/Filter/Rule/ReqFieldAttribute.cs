using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Filter.Rule
{
    /// <summary>
    /// 单个字段条件
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReqFieldAttribute : Attribute, IReqFieldAttribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; protected set; }

        /// <summary>
        /// 操作符
        /// </summary>
        public FilterOperate Op { get; protected set; }

        /// <summary>
        /// 条件连接
        /// </summary>
        public ConditionLink Link { get; protected set; }


        public ReqFieldAttribute(string fieldName)
            :this(fieldName, FilterOperate.Equal)
        { 
        }

        public ReqFieldAttribute(string fieldName, FilterOperate op)
            :this(fieldName, op, ConditionLink.And)
        {
        }


        public ReqFieldAttribute(string fieldName = "", 
            FilterOperate op = FilterOperate.Equal,
            ConditionLink link = ConditionLink.And)
        {
            FieldName = fieldName;
            Op = op;
            Link = link;
        }

        public virtual IReqRule ParseRule([NotNull]string fieldName, [NotNull]object value)
        {
            return new ReqRule(FieldName.IsNullOrWhiteSpace()? fieldName: FieldName, value, Op, Link);
        }
    }
}
