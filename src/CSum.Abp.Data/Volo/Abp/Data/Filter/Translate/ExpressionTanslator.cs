using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Data.Filter.Group;
using Volo.Abp.Data.Filter.Rule;

namespace Volo.Abp.Data.Filter.Translate
{
    public class ExpressionTanslator: ITranslator
    {
        protected string prefix = "csx";
        public TranslateResult Translate(TranslateContext context)
        {
            StringBuilder builder = new StringBuilder();
            var groups = context.FilterGroup.OrderBy(p => p.Link).ToList();
            if (groups.Count > 0)
            {
                builder.Append($" {prefix}=> ");
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
            }
            return new TranslateResult(builder.ToString(), new List<FilteringParam>());
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
            var field = $"{prefix}.{rule.Field}";
            var result = "";
            switch (rule.Operate)
            {
                case FilterOperate.StartsWith:
                    result = $"{field}.StartsWith({FormatValue(rule.Value)})";
                    break;
                case FilterOperate.EndsWith:
                    result = $"{field}.EndsWith({FormatValue(rule.Value)})";
                    break;
                case FilterOperate.Contains:
                    result = $"{field}.Contains({FormatValue(rule.Value)})";
                    break;
                default:
                    var opCode = rule.Operate.ToOperateCode();               
                    if (rule.Operate == FilterOperate.Equal)
                    { 
                        opCode = "=="; // expression, 得用 == 符号
                    }
                    var type = !rule.ConversionType.IsNullOrEmpty() ? "(" + rule.ConversionType + ")" : "";
                    var vType = rule.Value.GetType();
                    if (vType == typeof(Guid))
                    {
                        result = $"{type}{field}.ToString() {opCode} {FormatValue(rule.Value)}";
                    }
                    else
                    {
                        result = $"{type}{field}{opCode} {FormatValue(rule.Value)}";
                    }
                    break;
            };
            return result;
        }

        protected virtual string FormatReqBetweenRule(ReqBetweenRule rule)
        {
            var field = $"{prefix}.{rule.Field}";
            //如果是时间范围查询
            DateTimeOffset startDate; DateTimeOffset endDate;
            if (DateTimeOffset.TryParse(rule.StartValue.ToString(), out startDate) && DateTimeOffset.TryParse(rule.EndValue.ToString(), out endDate))
            {
                return $"{field} >= Convert.ToDateTime('{startDate.ToString("yyyy-MM-dd 00:00:00")}') && {field} <= Convert.ToDateTime('{endDate.ToString("yyyy-MM-dd 23:59:59")}')";
            }
            else
            {
                return $"{field} >= {rule.StartValue} && {field} <= {rule.EndValue}";
            }
        }

        protected virtual string ParseConditionLink(ConditionLink link)
        {
            string linkStr = "";
            switch (link)
            {
                case ConditionLink.And:
                    linkStr = " && ";
                    break;
                case ConditionLink.Or:
                    linkStr = " || ";
                    break;
                default:
                    linkStr = " && ";
                    break;
            }
            return linkStr;
        }

        protected virtual string FormatValue(object value)
        {
            var result = value.ToString(); //默认是int,long
            var vType = value.GetType();
            if (vType == typeof(string) || vType == typeof(Guid))
            {
                result = $"\"{value}\"";
            }
            else if (vType == typeof(bool))
            {
                result = $"bool.Parse(\"{value}\")";
            }
            return result;
        }
    }
}
