using System;
using System.Collections.Generic;
using System.Linq;

namespace Volo.Abp.Core
{
    /// <summary>
    /// 模块API分组选项
    /// </summary>
    public class AbpModuleApiGroupOptions
    {
        /// <summary>
        /// 分组
        /// </summary>
        public List<ModuleApiGroupModel> Groups { get; set; }

        /// <summary>
        /// xml文档集合
        /// </summary>
        public List<string> XmlDocuments { get; }

        public AbpModuleApiGroupOptions()
        {
            Groups = new List<ModuleApiGroupModel>();
            XmlDocuments = new List<string>();
        }

        public void Add(ModuleApiGroupModel module)
        {
            Add(module, module.Assembly.GetName().Name);
        }

        public void Add(ModuleApiGroupModel module, string xmlname)
        {
            Groups.Add(module);
            XmlDocuments.Add(xmlname);
        }

        public void AddXmlDocument(Type type)
        {
            XmlDocuments.Add(type.GetAssemblyName());
        }

        /// <summary>
        /// 获取类所属分组名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetGroupName(Type type)
        {
            var name = type.GetAssemblyName();
            var module = Groups.FirstOrDefault(p => p.Assembly.GetName().Name == name);
            if (module != null)
            {
                return module.Name;
            }
            else
            {
                return name.Split('.').First().ToLower();
            }
        }
    }
}
