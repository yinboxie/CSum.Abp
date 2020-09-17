namespace System
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 枚举项的描述信息
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举项的描述信息。</returns>
        public static string GetDescription(this Enum value)
        {
            var enumType = value.GetType();
            // 获取枚举常数名称。
            var name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                var fieldInfo = enumType.GetField(name);
                return fieldInfo.GetDescription();
            }

            return string.Empty;
        }
    }
}
