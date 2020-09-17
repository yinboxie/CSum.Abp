using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Tree
{
    public static class TreeHelper
    {
        /// <summary>
        /// 树形数据格式化
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ICollection<TreeModel> ToTree(ICollection<TreeModel> list)
        {
            //TODO:可以考虑支持转换成泛型树形结构数据 ToTree<T>

            var childrenMap = new Dictionary<string, List<TreeModel>>();
            var parentMap = new Dictionary<string, TreeModel>();
            var res = new List<TreeModel>();

            //首先按照
            foreach (var node in list)
            {
                node.HasChildren = false;
                node.Complete = true;
                // 注册子节点映射表
                if (!childrenMap.ContainsKey(node.ParentId))
                    childrenMap.Add(node.ParentId, new List<TreeModel>());
                else if (parentMap.ContainsKey(node.ParentId))
                {
                    parentMap[node.ParentId].HasChildren = true;
                    parentMap[node.ParentId].Leaf = false;
                }
                childrenMap[node.ParentId].Add(node);
                // 注册父节点映射节点表
                parentMap.Add(node.Id, node);

                // 查找自己的子节点
                if (!childrenMap.ContainsKey(node.Id))
                    childrenMap.Add(node.Id, new List<TreeModel>());
                else
                {
                    node.HasChildren = true;
                    node.Leaf = false;
                }
                node.ChildNodes =  childrenMap[node.Id];
            }

            // 获取祖先数据列表
            foreach (var item in childrenMap)
                if (!parentMap.ContainsKey(item.Key))
                    res.AddRange(item.Value);
            UpdateLevel(res, 1);
            return res;
        }

        private static void UpdateLevel(ICollection<TreeModel> data, int level)
        {
            foreach (var item in data)
            {
                item.Level = level;
                if (item.ChildNodes != null && item.ChildNodes.Count > 0)
                {
                    UpdateLevel(item.ChildNodes, level + 1);
                }
                else
                {
                    item.ChildNodes = null;
                }    
            }
        }
    }
}
