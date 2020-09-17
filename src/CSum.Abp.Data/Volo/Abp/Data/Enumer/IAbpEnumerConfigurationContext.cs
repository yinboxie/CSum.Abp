using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Enumer
{
    public interface IAbpEnumerConfigurationContext
    {
        Dictionary<string,Dictionary<int,string>> Enums { get; }

        Dictionary<string, string> EnumTypes { get; }

        IServiceProvider ServiceProvider { get; }
    }
}
