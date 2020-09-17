using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Domain.Entities
{

    /// <summary>
    /// 基础资料实体
    /// </summary>
    [Serializable]
    public abstract class SuperInfoEntity<TKey> : AuditedAggregateRoot<TKey>, IHasSimpleSpelling, IPassivable, ISortable
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty]
        public override TKey Id { get; protected set; }

        /// <summary>
        /// 编码
        /// </summary>
        [JsonProperty]
        [DisplayName("编码")]
        [MaxLength(128)]
        public virtual string Code { get; protected set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty]
        [DisplayName("名称")]
        [MaxLength(256)]
        [Required]
        public virtual string Name { get; protected set; }

        /// <summary>
        /// 助记码
        /// </summary>
        [DisplayName("助记码")]
        [MaxLength(256)]
        public virtual string SimpleSpelling { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty]
        [DisplayName("是否有效")]
        public virtual bool IsActive { get; protected set; }

        /// <summary>
        /// 排序码，数字越小越靠前
        /// </summary>
        [DisplayName("排序码")]
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        [MaxLength(512)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 其他属性
        /// </summary>
        [JsonConverter(typeof(CollectionConverter))]
        public override Dictionary<string, object> ExtraProperties { get; protected set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="code">编码</param> 
        public SuperInfoEntity([NotNull] string name, string code)
        {
            //this.Id = (TKey)Convert.ChangeType(Guid.NewGuid(), typeof(TKey));
            Name = Check.NotNullOrEmpty(name, nameof(name), 256);
            Code = code;

            IsActive = true;
            SortCode = 1;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="code"></param>
        public void SetCode(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 设置名称
        /// </summary>
        /// <param name="name"></param>
        public void SetName([NotNull] string name)
        {
            Name = Check.NotNullOrEmpty(name, nameof(name), 256);
        }
        /// <summary>
        /// 切换有效状态
        /// </summary>
        public void ToggleActive(bool? isActive = null)
        {
            if (isActive is null)
            {
                IsActive = !IsActive;
            }
            else
            {
                IsActive = isActive.Value;
            }
        }
        /// <summary>
        /// 获取拼音助记码名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetSimpleSpellingName()
        {
            return Name;
        }
        #endregion
    }
}
