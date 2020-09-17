using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.EntityFrameworkCore
{
    public static class CSumDbFunctions
    {
        /// <summary>
        /// 从byte[]类型转换成bigint, ef转换成sql为 Convert(bigint,value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ConvertToInt64(byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
