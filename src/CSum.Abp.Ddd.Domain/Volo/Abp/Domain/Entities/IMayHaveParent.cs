using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Domain.Entities
{
    /// <summary>
    /// 父级节点，约束作用
    /// </summary>
    public interface IMayHaveParent<TKey>
    {
        /// <summary>
        /// 父级节点主键
        /// </summary>
        TKey ParentId { get; set; }
    }

    /// <summary>
    /// 父级节点，框架约定继承此接口的实体主键必须是Guid类型
    /// </summary>
    public interface IMayHaveParent : IMayHaveParent<Guid?>
    {
    }
}
