using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Modularity;

namespace Volo.Abp.AspNetCore.Mvc
{
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class CSumAbpAspNetCoreMvcModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //错误信息转换
            context.Services.Replace(ServiceDescriptor.Transient<IExceptionToErrorInfoConverter, CSumExceptionToErrorInfoConverter>());

            //使用自定义的AbpServiceConvention
            context.Services.Replace(ServiceDescriptor.Transient<IAbpServiceConvention, CSumAbpServiceConvention>());

            //增加路由分组转换
            Configure<MvcOptions>(c =>
            {
                c.Conventions.Add(new RouteGroupConvention());
            });
        }
    }
}
