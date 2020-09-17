namespace Volo.Abp.Domain.Entities
{
    /// <summary>
    /// This interface is used to make an entity active/passive.
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// 是否有效，不能外部设置
        /// </summary>
        bool IsActive { get; }

        void ToggleActive(bool? isActive = null);
    }
}
