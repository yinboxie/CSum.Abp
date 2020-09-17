using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using Volo.Abp.Modularity;
using Volo.Abp.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;

namespace Volo.Abp.SwaggerUI
{
    public class CSumAbpSwaggerUIModule : AbpModule
    {
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //配置Swagger;
            var basePath = Path.GetDirectoryName(typeof(CSumAbpSwaggerUIModule).Assembly.Location);

            var groupOptionsLazy = context.Services.GetRequiredServiceLazy<IOptions<AbpModuleApiGroupOptions>>();

            Configure<SwaggerGenOptions>(c =>
            {
                var groupOptions = (groupOptionsLazy.Value).Value;

                //为 Swagger 添加 Bearer Token 认证
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",  //这个名称必须和上方定义的名称一致
                                Type = ReferenceType.SecurityScheme
                            }
                        },Array.Empty<string>()}
                    });

                c.IgnoreObsoleteActions(); //忽略已过时api

                //增加服务端  必须得有
                var local = new OpenApiServer()
                {
                    Url = "",
                    Description = "本地系统"
                };
                c.AddServer(local);
                //自定义操作id 必须得有
                c.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return controllerAction.ControllerName + "-" + controllerAction.ActionName;
                });

                //加载分组
                foreach (var group in groupOptions.Groups)
                {
                    c.SwaggerDoc(group.Name, new OpenApiInfo { Title = group.Title, Version = group.Name });
                }

                //加载xml
                foreach (var xmlname in groupOptions.XmlDocuments)
                {
                    var apiXmlPath = Path.Combine(basePath, $"{xmlname}.xml");
                    if (File.Exists(apiXmlPath))
                    {
                        c.IncludeXmlComments(apiXmlPath, true);
                    }
                }
            });
        }
    }
}
