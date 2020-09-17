using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Core;
using Microsoft.AspNetCore.Builder;

namespace Volo.Abp.SwaggerUI
{
    public static class CSumAbpSwaggerUIOptionsExtensions
    {
        /// <summary>
        /// 扩展的Abp Swagger
        /// </summary>
        /// <param name="options"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="appPath">发布的目录</param>
        public static void AbpSwaggerUI(this CSumAbpSwaggerUIOptions options, IServiceProvider serviceProvider, string appPath = "")
        {
            //options.RoutePrefix = ""; //默认路由前缀是swagger
            options.IndexStream = () => typeof(CSumAbpSwaggerUIOptionsExtensions).GetTypeInfo().Assembly
            .GetManifestResourceStream("doc.html");
            options.AppPath = appPath;

            var groupOptions = serviceProvider.GetRequiredService<IOptions<AbpModuleApiGroupOptions>>().Value;

            foreach (var item in groupOptions.Groups)
            {
                var path = appPath.IsNullOrWhiteSpace() ? "" :( "/" + appPath);
                var rfix = options.RoutePrefix.IsNullOrWhiteSpace() ? "" : (options.RoutePrefix + "/");
                options.SwaggerEndpoint($"{path}/{rfix}{item.Name}/swagger.json",item.Title);
            }
        }
    }
}
