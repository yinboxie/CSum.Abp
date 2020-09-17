using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Enumer
{
    public class AbpEnumerConfigurationContext : IAbpEnumerConfigurationContext
    {
        public Dictionary<string, Dictionary<int, string>> Enums { get; }

        public Dictionary<string, string> EnumTypes { get; }

        public IServiceProvider ServiceProvider { get; }

        public AbpEnumerConfigurationContext(
            Dictionary<string, Dictionary<int, string>> enums,
            IServiceProvider serviceProvider)
        {
            Enums = enums;
            EnumTypes = new Dictionary<string, string>();
            ServiceProvider = serviceProvider;
        }
    }
}
