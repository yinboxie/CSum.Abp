namespace Volo.Abp.Domain.Entities
{
    /// <summary>
    /// 助记码生成接口
    /// </summary>
    public interface IHasSimpleSpelling
    {
        /// <summary>
        /// 获取生成助记码的名称内容
        /// </summary>
        /// <returns></returns>
        string GetSimpleSpellingName();

        /// <summary>
        /// 名称拼音首字母
        /// </summary>
        string SimpleSpelling { get; set; }
    }
}
