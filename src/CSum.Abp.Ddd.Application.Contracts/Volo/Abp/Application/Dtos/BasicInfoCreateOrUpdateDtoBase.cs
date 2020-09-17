using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    /// <summary>
    /// 基础资料(添加或更新基类)
    /// </summary>
    public abstract class BasicInfoCreateOrUpdateDtoBase
    {
        #region 属性
        /// <summary>
        /// 编码
        /// </summary>
        [MaxLength(128)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required,MaxLength(256)]
        public virtual string Name { get; set; }

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
        [MaxLength(512)]
        public virtual string Remark { get; set; }
        #endregion
    }
}
