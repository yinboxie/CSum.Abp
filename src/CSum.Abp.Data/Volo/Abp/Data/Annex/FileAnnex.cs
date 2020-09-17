using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Annex
{
    /// <summary>
    /// 文件附件
    /// </summary>
    public class FileAnnex 
    {
        #region 属性
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">文件路径</param>
        public FileAnnex([CanBeNull] string name, [NotNull]string path)
        {
            Path = Check.NotNullOrEmpty(path, nameof(path));
            Size = 0;
            Name = name.IsNullOrEmpty() ? System.IO.Path.GetFileName(path) : name;
            Type = System.IO.Path.GetExtension(path).Substring(1);
        }
    }
}
