using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Volo.Abp.Core
{
    /// <summary>
    /// 模块Api分组模型
    /// </summary>
    public class ModuleApiGroupModel
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模块所在程序集
        /// </summary>
        public Assembly Assembly { get; set; }

        public ModuleApiGroupModel([NotNull]string name, string title, [NotNull] Assembly assembly)
        {
            Name = Check.NotNullOrEmpty(name, nameof(name));
            Title = title.IsNullOrEmpty()? name : title;
            Assembly = assembly;
        }
    }
}
