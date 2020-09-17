using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Data.Enumer
{
    internal class EnumerAccessor : IEnumerAccessor
    {
        public Dictionary<string, Dictionary<int, string>> Enumer { get; set; }

        public Dictionary<string, string> AllEnumType { get; set; }
    }
}
