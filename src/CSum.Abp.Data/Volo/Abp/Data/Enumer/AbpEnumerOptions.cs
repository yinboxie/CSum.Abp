using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Volo.Abp.Data.Enumer
{
    public class AbpEnumerOptions
    {
        public List<Action<IAbpEnumerConfigurationContext>> Configurators { get; }

        public List<string> IgnoreEnums { get; set; }

        public AbpEnumerOptions()
        {
            Configurators = new List<Action<IAbpEnumerConfigurationContext>>();
            IgnoreEnums = new List<string>();
        }

        public void Add<TModule>()
        {
            Add<TModule>(null, null);
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="func">委托返回所有的枚举项值</param>
        /// <param name="func2">委托返回所有的枚举类型</param>
        public void Add<TModule>(Func<Assembly, Dictionary<string, Dictionary<int, string>>> func,
            Func<Assembly, Dictionary<string, string>> func2)
        {
            var assembly = typeof(TModule).Assembly;

            Configurators.Add(context =>
            {
                Dictionary<string, Dictionary<int, string>> dic = new Dictionary<string, Dictionary<int, string>>();
                Dictionary<string, string> dicType = new Dictionary<string, string>();
                if (func != null)
                {
                    dic = func(assembly);
                    if (func2 == null)
                    {
                        throw new Exception("返回枚举类型的委托不能为空");
                    }
                    dicType = func2(assembly);
                }
                else
                {
                    var types = assembly.GetTypes().Where(p => p.IsEnum);
                    foreach (var enumType in types)
                    {
                        FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                        var rs = new Dictionary<int, string>();
                        foreach (var item in fields)
                        {
                            var fieldValue = item.GetRawConstantValue();
                            var fieldDescription = item.GetDescription();
                            rs.Add((int)fieldValue, fieldDescription);
                        }

                        dic.Add(enumType.Name, rs);
                        dicType.Add(enumType.Name, enumType.GetDescription());
                    }
                }
                foreach (var item in dic)
                {
                    if (!context.Enums.ContainsKey(item.Key)) 
                    {
                        context.Enums.Add(item.Key, item.Value);
                    }
                    
                }
                foreach (var item in dicType)
                {
                    if (!context.EnumTypes.ContainsKey(item.Key))
                    {
                        context.EnumTypes.Add(item.Key, item.Value);
                    }
                }
            });
        }

        public void Ignore<TEnum>()
        {
            IgnoreEnums.Add(typeof(TEnum).Name);
        }
    }
}
