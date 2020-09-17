using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Domain.Entities
{
    /// <summary>
    /// 是否有行版本，实体继承之后,会使用x=>x.IsRowVersion()指定关系，会作为并发标记
    /// </summary>
    public interface IHasRowVersion
    {
        /// <summary>
        /// 行版本号,对应sql server数据库的timestamp类型字段
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
