using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Enumer
{
    public interface IEnumerAccessor
    {
        Dictionary<string, Dictionary<int, string>> Enumer { get; }
        Dictionary<string, string> AllEnumType { get; }
    }
}
