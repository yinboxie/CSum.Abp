using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using System.Linq;
using Volo.Abp.Data.Filter.Rule;
using Volo.Abp.Data.Filter.Translate;
using Volo.Abp.Data.Filter.Group;
using System.Reflection;

namespace Volo.Abp.Data.Filter
{
    /// <summary>
    /// 筛选条件服务
    /// </summary>
    public class FilterService : IFilterService,ITransientDependency
    {
        protected readonly CSumAbpFilterOptions Options;

        protected FilteringCondition Condition;
        public FilterService(IOptions<CSumAbpFilterOptions> options)
        {
            Options = options.Value;
        }

        public virtual FilteringCondition Translate(IFilterModel model)
        {
            return Translate(model, Options.DefaultTranslator);
        }

        public virtual FilteringCondition Translate(IFilterModel model, ITranslator translator)
        {
            Condition = new FilteringCondition(model);
            //设置额外属性
            SetConditionExtraProperties(TryParseExtraProperties(model));


            //解析成ReqGroupList
            var reqGroups = TryParseReqGroupList(model);

            //翻译
            var result = translator.Translate(new TranslateContext { DbParameterPrefix = Options.DbParameterPrefix, FilterGroup = reqGroups });

            SetConditionConditionalText(result.ConditionalText);
            SetConditionFilteringParams(result.FilteringParams);
            return Condition;
        }

        /// <summary>
        /// 解析成<see cref="ReqGroupList"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual ReqGroupList TryParseReqGroupList(IFilterModel model)
        {
            ReqGroupList reqGroups = new ReqGroupList();
            var properties = model.GetType().GetProperties();

            //获取标记了IFilterFieldAttribute特性的字段
            var filterFields = properties.Where(p => p.CustomAttributes.Any(a => typeof(IReqFieldAttribute).IsAssignableFrom(a.AttributeType)));

            //获取标记了ReqGroupAttribute特性的字段
            var groupFields = filterFields.Where(p => p.CustomAttributes.Any(a => typeof(IReqGroupAttribute).IsAssignableFrom(a.AttributeType)));
            Dictionary<int, ConditionLink> dLinkPairs = new Dictionary<int, ConditionLink>();
            Dictionary<int, List<PropertyInfo>> dPropertyPairs = new Dictionary<int, List<PropertyInfo>>();
            foreach (var gField in groupFields)
            {
                //将相同分组序号的放置一个ReqGroup, 并且必须得有相同的Link
                IReqGroupAttribute attr = (IReqGroupAttribute)gField.GetCustomAttributes().FirstOrDefault(a => typeof(IReqGroupAttribute).IsAssignableFrom(a.GetType()));
                if (!dLinkPairs.ContainsKey(attr.SortCode))
                {
                    dLinkPairs.Add(attr.SortCode, attr.Link);
                    var ls = new List<PropertyInfo>();
                    ls.Add(gField);
                    dPropertyPairs.Add(attr.SortCode, ls);
                }
                else
                {
                    if (dLinkPairs[attr.SortCode] != attr.Link)
                    {
                        throw new Exception($"解析过滤规则分组失败，相同组序号({attr.SortCode})必须有相同的Link连接符");
                    }
                    dPropertyPairs[attr.SortCode].Add(gField);
                }
            }
            foreach (var fields in dPropertyPairs.Values)
            {
                var onegroup = TryGetReqGroupWithProperties(model, fields);
                reqGroups.AddGroup(onegroup);
            }


            //将没有标记ReqGroupAttribute特性放置在一组
            var noGroupFields = filterFields.Where(p => !p.CustomAttributes.Any(a => typeof(IReqGroupAttribute).IsAssignableFrom(a.AttributeType)));
            var group = TryGetReqGroupWithProperties(model, noGroupFields);
            if(group.Rules.Count > 0) reqGroups.AddGroup(group);
            return reqGroups;
        }

        /// <summary>
        /// 将模型<see cref="IFilterModel"/>的属性集解析成<see cref="IReqGroup"/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="reqFields"></param>
        /// <returns></returns>
        protected virtual IReqGroup TryGetReqGroupWithProperties(IFilterModel model, IEnumerable<PropertyInfo> reqFields)
        {
            ReqGroup group = new ReqGroup();
            foreach (var field in reqFields)
            {
                var val = field.GetValue(model);
                if (val == null) continue;

                IReqFieldAttribute attr = (IReqFieldAttribute)field.GetCustomAttributes().FirstOrDefault(a => typeof(IReqFieldAttribute).IsAssignableFrom(a.GetType()));

                
                var rule = attr.ParseRule(field.Name,val);
                if (field.PropertyType.IsNullableEnum() && rule is ReqRule reqRule)
                {
                    //枚举字段需要转换成int
                    reqRule.SetConversionType(typeof(int), "int");
                }
                group.AddRule(rule);
            }
            return group;
        }

        /// <summary>
        /// 解析未标注<see cref="IReqFieldAttribute"/>的字段
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual Dictionary<string, object> TryParseExtraProperties(IFilterModel model)
        {
            var properties = model.GetType().GetProperties();
            var fields = properties.Where(p => !p.CustomAttributes.Any(a => typeof(IReqFieldAttribute).IsAssignableFrom(a.AttributeType)));
            var extraProperties = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                var val = field.GetValue(model);
                if (val != null && !val.ToString().IsNullOrEmpty())
                {
                    extraProperties.Add(field.Name, field.GetValue(model));
                }
            }
            return extraProperties;
        }


        #region 设置 FilteringCondition 属性
        protected virtual void SetConditionExtraProperties(Dictionary<string, object> extraProperties)
        {
            Condition.SetExtraProperties(extraProperties);
        }

        protected virtual void SetConditionFilteringParams(IEnumerable<FilteringParam> filteringParams)
        {
            Condition.SetFilteringParams(filteringParams);
        }

        protected virtual void SetConditionConditionalText(string conditionalText)
        {
            Condition.SetConditionalText(conditionalText);
        }
        #endregion       
    }
}
