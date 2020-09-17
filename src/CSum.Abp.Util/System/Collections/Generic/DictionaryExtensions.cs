using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 字典类型转化为动态对象
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static dynamic ToDynamic(this IDictionary<string, object> dict)
        {
            dynamic result = new System.Dynamic.ExpandoObject();

            foreach (var entry in dict)
            {
                (result as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(entry.Key, entry.Value));
            }

            return result;
        }

        /// <summary>
        /// 字典类型转化为实体对象
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this IDictionary<string, object> dic) where T : new()
        {
            var md = new T();
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                var filed = textInfo.ToTitleCase(d.Key);
                var value = d.Value;
                var pro = md.GetType().GetProperty(filed);
                if (pro != null)
                    pro.SetValue(md, value);
            }
            return md;
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
