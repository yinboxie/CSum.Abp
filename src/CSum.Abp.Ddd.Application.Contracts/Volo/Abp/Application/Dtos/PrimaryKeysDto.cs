using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    public class PrimaryKeysDto<TKey>
    {
        /// <summary>
        /// 主键集合
        /// </summary>
        public List<TKey> Ids { get; set; }
    }
}
