using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data.Filter.Group;
using Volo.Abp.Data.Filter.Rule;
using System.Linq;

namespace Volo.Abp.Data.Filter.Translate
{
    public class SqlTranslator : ITranslator
    {
        protected List<FilteringParam> FilteringParams;
        protected TranslateContext Context;

        public TranslateResult Translate(TranslateContext context)
        {
            FilteringParams = new List<FilteringParam>();
            Context = context;
            var groups = context.FilterGroup.OrderBy(p => p.Link).ToList();
            StringBuilder builder = new StringBuilder();
            if (groups.Count > 0)
            {
                builder.Append(" and ( ");
                //校验第1个分组，条件连接符不能为or
                if (groups[0].Link == ConditionLink.Or)
                {
                    throw new Exception($"解析sql脚本失败，第一个分组条件连接符不能为 Or");
                }
                for (int i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    if (i != 0)
                    {
                        builder.Append(ParseConditionLink(group.Link));
                    }
                    builder.Append(ParseReqRuleListToSql(group.Rules.OrderBy(p => p.Link).ToList()));
                }

                builder.Append(" )");
            }           
            return new TranslateResult(builder.ToString(), FilteringParams);
        }

        protected virtual string ParseReqRuleListToSql(IList<IReqRule> rules, IReqRule parentRule = null)
        {
            StringBuilder sqlStr = new StringBuilder(" ( ");
            if (!(parentRule is ReqMultiRule) && rules.Count > 0 && rules[0].Link == ConditionLink.Or)
            {
                throw new Exception($"解析sql脚本失败，分组的第一个过滤条件连接符不能为 Or");
            }

            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (i != 0)
                {
                    sqlStr.Append(ParseConditionLink(rule.Link));
                }
                sqlStr.Append(ParseReqRuleToSql(rule));
            }

            sqlStr.Append(" ) ");
            return sqlStr.ToString();
        }

        protected virtual string ParseReqRuleToSql(IReqRule rule)
        {
            if (rule is ReqRule reqRule)
            {
                return FormatReqRule(reqRule);
            }
            else if (rule is ReqMultiRule multiRule)
            {
                return ParseReqRuleListToSql(multiRule.Rules.ToList(), multiRule);
            }
            else if (rule is ReqBetweenRule betweenRule)
            {
                return FormatReqBetweenRule(betweenRule);
            }
            throw new Exception($"无法解析的过滤规则({rule.GetType().FullName})");
        }

        protected virtual string FormatReqRule(ReqRule rule)
        {
            var paramName = AddFilteringParam(rule.Value);
            if (rule.Operate == FilterOperate.StartsWith)
            {
                return $"{rule.Field} like '{paramName}%'";
            }
            else if (rule.Operate == FilterOperate.EndsWith)
            {
                return $"{rule.Field} like '%{paramName}'";
            }
            else if (rule.Operate == FilterOperate.Contains)
            {
                return $"{rule.Field} like '%{paramName}%'";
            }
            else
            {
                var opCode = rule.Operate.ToOperateCode();
                var vtype = rule.Value.GetType(); // 值类型
                if (vtype == typeof(string))
                {
                    return $"{rule.Field} {opCode} '{paramName}'";
                }
                else
                {
                    return $"{rule.Field} {opCode} {paramName}";
                }
            }
        }

        protected virtual string FormatReqBetweenRule(ReqBetweenRule rule)
        {
            //如果是时间范围查询
            DateTimeOffset startDate; DateTimeOffset endDate;
            if (DateTimeOffset.TryParse(rule.StartValue.ToString(), out startDate) && DateTimeOffset.TryParse(rule.EndValue.ToString(), out endDate))
            {
                return $"{rule.Field} between '{startDate.ToString("yyyy-MM-dd 00:00:00")}' and '{endDate.ToString("yyyy-MM-dd 23:59:59")}'";
            }
            else
            {
                return $"{rule.Field} between {rule.StartValue} and {rule.EndValue}";
            }
        }

        protected virtual string AddFilteringParam(object value)
        {
            var paramName = $"{Context.DbParameterPrefix}Field_{FilteringParams.Count}";
            FilteringParams.Add(new FilteringParam(paramName, value));
            return paramName;
        }

        protected virtual string ParseConditionLink(ConditionLink link) 
        {
            string linkStr = "";
            switch (link)
            {
                case  ConditionLink.And :
                    linkStr = " and ";
                    break;
                case ConditionLink.Or:
                    linkStr = " or ";
                    break;
                default:
                    linkStr = " and ";
                    break;
            }
            return linkStr;
        }
    }
}
