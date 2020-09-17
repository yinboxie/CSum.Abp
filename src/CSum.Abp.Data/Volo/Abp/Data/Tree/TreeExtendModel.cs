using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Tree
{
    /// <summary>
    /// 树形-扩展模型
    /// </summary>
    public class TreeExtendModel
    {
        /// <summary>
        /// 展开的键
        /// </summary>
        public List<string> ExpendKeys { get; set; }

        /// <summary>
        /// 选择的键
        /// </summary>
        public List<string> CheckedKeys { get; set; }

        /// <summary>
        /// 树形展示数据
        /// </summary>
        public ICollection<TreeModel> Items { get; set; }
    }
}
