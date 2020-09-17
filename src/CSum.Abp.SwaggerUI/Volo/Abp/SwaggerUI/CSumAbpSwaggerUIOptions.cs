using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.SwaggerUI
{
    public class CSumAbpSwaggerUIOptions : SwaggerUIOptions
    {
        /// <summary>
        /// 站点发布路径
        /// </summary>
        public string AppPath { get; set; }
    }
}
