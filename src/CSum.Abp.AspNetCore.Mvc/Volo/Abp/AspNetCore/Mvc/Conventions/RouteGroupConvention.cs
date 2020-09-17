using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Volo.Abp.AspNetCore.Mvc.Conventions
{
    public class RouteGroupConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _centralPrefix;

        public RouteGroupConvention()
        {
            _centralPrefix = new AttributeRouteModel();
        }

        //接口的Apply方法
        public void Apply(ApplicationModel application)
        {
            //遍历所有的 Controller
            foreach (var controller in application.Controllers)
            {
                var apiVersion = controller.ApiExplorer.GroupName;

                // 已经标记了 RouteAttribute 的 Controller
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        // 在 当前路由上 再 添加一个 路由前缀
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(new AttributeRouteModel(new RouteAttribute("api/" + apiVersion + "/")),
                            selectorModel.AttributeRouteModel);
                    }
                }
                else
                {
                    // 没有标记 RouteAttribute 的 Controller
                    var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                    if (unmatchedSelectors.Any())
                    { 
                        foreach (var selectorModel in unmatchedSelectors)
                        {
                            // 在 当前路由上 再 添加一个 路由前缀
                            selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(new AttributeRouteModel(new RouteAttribute("api/" + apiVersion + "/")),
                                new AttributeRouteModel() { Template = controller.ControllerName.ToLower() + "s" });
                            //// 添加一个 路由前缀
                            //selectorModel.AttributeRouteModel = _centralPrefix;
                        }
                    }
                }
            }
        }
    }
}
