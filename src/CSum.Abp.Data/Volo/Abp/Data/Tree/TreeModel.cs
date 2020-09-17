using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Tree
{
    /// <summary>
    /// 树形结构模型
    /// </summary>
    public class TreeModel
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 节点显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 节点值内容
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// 节点文本标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 是否显示勾选框
        /// </summary>
        public bool ShowCheck { get; set; } = false;

        /// <summary>
        /// 是否被勾选 0 未勾选, 1 部分勾选, 2 全部勾选
        /// </summary>
        public int CheckState { get; set; } = 0;

        /// <summary>
        /// 子节点是否已经加载完成了
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpand { get; set; } = true;

        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 是否为叶子节点
        /// </summary>
        public bool Leaf { get; set; } = true;

        /// <summary>
        /// 显示图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///  级数
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 节点数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 是否可勾选
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// 插槽
        /// </summary>
        public object ScopedSlots { get; set; } = new { icon= "iconSlot",title="titleSlot" };

        /// <summary>
        /// 子节点列表数据
        /// </summary>
        public ICollection <TreeModel> ChildNodes { get; set; }
    }
}
