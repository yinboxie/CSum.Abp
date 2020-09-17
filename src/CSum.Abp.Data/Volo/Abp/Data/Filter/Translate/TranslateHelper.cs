using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Volo.Abp.Data.Filter.Translate
{
    public static class TranslateHelper
    {
        /// <summary>
        /// 把查询操作的枚举表示转换为操作码
        /// </summary>
        /// <param name="operate">查询操作的枚举表示</param>
        public static string ToOperateCode(this FilterOperate operate)
        {
            Type type = operate.GetType();
            MemberInfo[] members = type.GetMember(operate.CastTo<string>());
            if (members.Length == 0)
            {
                return null;
            }

            OperateAttribute attribute = members[0].GetAttribute<OperateAttribute>();
            return attribute?.Code;
        }
    }
}
