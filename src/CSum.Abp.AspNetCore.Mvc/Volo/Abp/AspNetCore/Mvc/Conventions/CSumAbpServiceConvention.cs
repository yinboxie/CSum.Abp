using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Core;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Modeling;
using Volo.Abp.Reflection;

namespace Volo.Abp.AspNetCore.Mvc.Conventions
{
    /// <summary>
    /// Abp服务转换
    /// </summary>
    public class CSumAbpServiceConvention: AbpServiceConvention
    {
        private readonly AbpModuleApiGroupOptions _groupOptions;

        public CSumAbpServiceConvention(IOptions<AbpAspNetCoreMvcOptions> options,
            IOptions<AbpModuleApiGroupOptions> groupOptions) 
            : base(options)
        {
            _groupOptions = groupOptions.Value;
        }
        protected override void ApplyForControllers(ApplicationModel application)
        {
            //给controller赋值分组名称
            foreach (var controller in application.Controllers)
            {
                controller.ApiExplorer.GroupName = _groupOptions.GetGroupName(controller.ControllerType);
            }
            base.ApplyForControllers(application);
        }

        protected override string GetRootPathOrDefault(Type controllerType)
        {
            var areaAttribute = controllerType.GetCustomAttributes().OfType<AreaAttribute>().FirstOrDefault();
            if (areaAttribute?.RouteValue != null)
            {
                return areaAttribute.RouteValue;
            }

            var controllerSetting = GetControllerSettingOrNull(controllerType);
            if (controllerSetting?.RootPath != null)
            {
                return controllerSetting.RootPath;
            }

            return ModuleApiDescriptionModel.DefaultRootPath;
        }
    }
}
