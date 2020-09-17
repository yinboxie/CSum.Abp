using System;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Application.Dtos
{
    /// <summary>
    /// 基础资料
    /// </summary>
    [Serializable]
    public abstract class BasicInfoDto<TKey> : AuditedEntityDto<TKey>, IHasConcurrencyStamp
    {
        #region 属性
        /// <summary>
        /// 编码
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 助记码
        /// </summary>
        public virtual string SimpleSpelling { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 并发标识戳
        /// </summary>
        public string ConcurrencyStamp { get; set; }
        #endregion
    }
}
