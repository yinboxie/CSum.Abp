using JetBrains.Annotations;

namespace Volo.Abp.Data.Filter.Rule
{
    /// <summary>
    /// 过滤字段约束
    /// </summary>
    public interface IReqFieldAttribute
    {
        /// <summary>
        /// 解析规则
        /// </summary>
        /// <param name="value">属性值</param>
        /// <param name="fieldName">默认字段名称，优先使用Attribute特性标注的名称</param>
        /// <returns></returns>
        IReqRule ParseRule([NotNull]string fieldName, [NotNull]object value);
    }
}
